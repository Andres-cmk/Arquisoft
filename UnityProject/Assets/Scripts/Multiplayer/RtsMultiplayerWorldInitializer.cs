using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RtsMultiplayerWorldInitializer : MonoBehaviour
{
    public static RtsMultiplayerWorldInitializer Instance { get; private set; }

    [SerializeField] float slotSpacing = 28f;
    [SerializeField] float resourceClearanceRadius = 10f;
    [SerializeField] float baseClearanceRadius = 18f;
    [SerializeField] float slotSearchStep = 7f;
    [SerializeField] int slotSearchRings = 10;
    [SerializeField] float navMeshSampleDistance = 3f;

    bool initialized;

    static readonly Vector3[] SlotOffsets =
    {
        Vector3.zero,
        new Vector3(18f, 0f, 0f),
        new Vector3(0f, 0f, 18f),
        new Vector3(18f, 0f, 18f),
    };

    public static RtsMultiplayerWorldInitializer GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<RtsMultiplayerWorldInitializer>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("RtsMultiplayerWorldInitializer");
        Instance = go.AddComponent<RtsMultiplayerWorldInitializer>();
        return Instance;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeForCurrentMatch()
    {
        if (initialized)
        {
            RtsEntityRegistry.RefreshAllLocalCategories();
            return;
        }

        StartCoroutine(InitializeAfterSceneStart());
    }

    IEnumerator InitializeAfterSceneStart()
    {
        initialized = true;
        RtsEntityRegistry.Clear();
        GameSessionStats.GetOrCreate().ResetSession();

        yield return null;
        yield return null;

        int playerCount = MultiplayerBootstrap.Instance != null ? MultiplayerBootstrap.Instance.GetPlayerCount() : 1;
        AssignResources();

        EdificioCentral[] baseBuildings = FindObjectsByType<EdificioCentral>(FindObjectsSortMode.None);
        abr[] baseUnits = FindObjectsByType<abr>(FindObjectsSortMode.None);

        AssignStartingBuildings(baseBuildings, 0, SlotOffsets[0]);
        AssignStartingUnits(baseUnits, 0, SlotOffsets[0]);

        int cappedPlayerCount = Mathf.Clamp(playerCount, 1, 4);
        ResourceNode[] resources = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
        Vector3 baseCenter = CalculateGroupCenter(baseBuildings, baseUnits);
        List<Vector3> occupiedBaseCenters = new List<Vector3> { baseCenter };

        for (int slot = 1; slot < cappedPlayerCount; slot++)
        {
            Vector3 offset = FindFreeSlotOffset(slot, baseCenter, baseBuildings, baseUnits, resources, occupiedBaseCenters);
            CloneStartingBuildings(baseBuildings, slot, offset);
            CloneStartingUnits(baseUnits, slot, offset);
            occupiedBaseCenters.Add(baseCenter + offset);
        }

        AssignStartingBuildings(baseBuildings, 0, SlotOffsets[0]);
        AssignStartingUnits(baseUnits, 0, SlotOffsets[0]);

        RtsEntityRegistry.RefreshAllLocalCategories();
        RtsNetworkCommandBus.GetOrCreate().Activate();
    }

    public void ResetInitializationState()
    {
        initialized = false;
        StopAllCoroutines();
    }

    void AssignResources()
    {
        foreach (ResourceNode resource in FindObjectsByType<ResourceNode>(FindObjectsSortMode.None))
        {
            RtsEntityRegistry.GetOrAdd(resource.gameObject, RtsEntityRegistry.BuildResourceId(resource), -1, RtsEntityKind.Resource);
        }
    }

    void AssignStartingBuildings(EdificioCentral[] buildings, int ownerSlot, Vector3 offset)
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            if (buildings[i] == null)
            {
                continue;
            }

            int entityId = 20000 + ownerSlot * 1000 + i + 1;
            RtsEntityRegistry.GetOrAdd(buildings[i].gameObject, entityId, ownerSlot, RtsEntityKind.Building);
        }
    }

    void AssignStartingUnits(abr[] units, int ownerSlot, Vector3 offset)
    {
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null)
            {
                continue;
            }

            int entityId = 10000 + ownerSlot * 1000 + i + 1;
            RtsEntityRegistry.GetOrAdd(units[i].gameObject, entityId, ownerSlot, RtsEntityKind.Unit);
        }
    }

    void CloneStartingBuildings(EdificioCentral[] templates, int ownerSlot, Vector3 offset)
    {
        for (int i = 0; i < templates.Length; i++)
        {
            EdificioCentral template = templates[i];
            if (template == null)
            {
                continue;
            }

            GameObject clone = Instantiate(template.gameObject, template.transform.position + offset, template.transform.rotation);
            clone.name = template.name + "_P" + ownerSlot;
            int entityId = 20000 + ownerSlot * 1000 + i + 1;
            RtsEntityRegistry.GetOrAdd(clone, entityId, ownerSlot, RtsEntityKind.Building);
        }
    }

    void CloneStartingUnits(abr[] templates, int ownerSlot, Vector3 offset)
    {
        for (int i = 0; i < templates.Length; i++)
        {
            abr template = templates[i];
            if (template == null)
            {
                continue;
            }

            GameObject clone = Instantiate(template.gameObject, template.transform.position + offset, template.transform.rotation);
            clone.name = template.name + "_P" + ownerSlot;
            int entityId = 10000 + ownerSlot * 1000 + i + 1;
            RtsEntityRegistry.GetOrAdd(clone, entityId, ownerSlot, RtsEntityKind.Unit);
        }
    }

    Vector3 GetSlotOffset(int slot)
    {
        if (slot >= 0 && slot < SlotOffsets.Length)
        {
            Vector3 preset = SlotOffsets[slot];
            return new Vector3(
                Mathf.Sign(preset.x) * Mathf.Max(Mathf.Abs(preset.x), slotSpacing),
                0f,
                Mathf.Sign(preset.z) * Mathf.Max(Mathf.Abs(preset.z), slotSpacing));
        }

        return new Vector3(slotSpacing * slot, 0f, 0f);
    }

    Vector3 FindFreeSlotOffset(
        int slot,
        Vector3 baseCenter,
        EdificioCentral[] baseBuildings,
        abr[] baseUnits,
        ResourceNode[] resources,
        List<Vector3> occupiedBaseCenters)
    {
        Vector3 preferredOffset = GetSlotOffset(slot);
        float step = GetSearchStep();
        int maxRings = Mathf.Max(0, slotSearchRings);

        for (int ring = 0; ring <= maxRings; ring++)
        {
            for (int x = -ring; x <= ring; x++)
            {
                for (int z = -ring; z <= ring; z++)
                {
                    if (ring > 0 && Mathf.Abs(x) != ring && Mathf.Abs(z) != ring)
                    {
                        continue;
                    }

                    Vector3 candidateOffset = preferredOffset + new Vector3(x * step, 0f, z * step);
                    if (IsSlotPlacementFree(candidateOffset, baseCenter, baseBuildings, baseUnits, resources, occupiedBaseCenters))
                    {
                        return candidateOffset;
                    }
                }
            }
        }

        Debug.LogWarning("[MULTIPLAYER] No se encontro un spawn libre para el jugador " + slot + ". Se usara el offset preferido.");
        return preferredOffset;
    }

    bool IsSlotPlacementFree(
        Vector3 candidateOffset,
        Vector3 baseCenter,
        EdificioCentral[] baseBuildings,
        abr[] baseUnits,
        ResourceNode[] resources,
        List<Vector3> occupiedBaseCenters)
    {
        Vector3 candidateCenter = baseCenter + candidateOffset;
        foreach (Vector3 occupiedCenter in occupiedBaseCenters)
        {
            if (HorizontalDistance(candidateCenter, occupiedCenter) < baseClearanceRadius)
            {
                return false;
            }
        }

        foreach (EdificioCentral building in baseBuildings)
        {
            if (building == null)
            {
                continue;
            }

            Vector3 targetPosition = building.transform.position + candidateOffset;
            if (!IsWorldPositionAvailable(targetPosition, resources, true))
            {
                return false;
            }
        }

        foreach (abr unit in baseUnits)
        {
            if (unit == null)
            {
                continue;
            }

            Vector3 targetPosition = unit.transform.position + candidateOffset;
            if (!IsWorldPositionAvailable(targetPosition, resources, false))
            {
                return false;
            }
        }

        return true;
    }

    bool IsWorldPositionAvailable(Vector3 position, ResourceNode[] resources, bool requireBuildingClearance)
    {
        if (!IsInsideGeneratedMap(position))
        {
            return false;
        }

        float clearance = requireBuildingClearance ? resourceClearanceRadius : resourceClearanceRadius * 0.5f;
        foreach (ResourceNode resource in resources)
        {
            if (resource == null)
            {
                continue;
            }

            if (HorizontalDistance(position, resource.transform.position) < clearance)
            {
                return false;
            }
        }

        if (NavMesh.SamplePosition(position, out _, navMeshSampleDistance, NavMesh.AllAreas))
        {
            return true;
        }

        return FindFirstObjectByType<MapGenerator>() == null;
    }

    bool IsInsideGeneratedMap(Vector3 position)
    {
        MapGenerator mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator == null)
        {
            return true;
        }

        float maxX = Mathf.Max(0f, (mapGenerator.mapWidth - 1) * mapGenerator.tileSize);
        float maxZ = Mathf.Max(0f, (mapGenerator.mapHeight - 1) * mapGenerator.tileSize);
        float padding = Mathf.Max(0f, resourceClearanceRadius);

        return position.x >= padding
            && position.z >= padding
            && position.x <= maxX - padding
            && position.z <= maxZ - padding;
    }

    Vector3 CalculateGroupCenter(EdificioCentral[] buildings, abr[] units)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (EdificioCentral building in buildings)
        {
            if (building == null)
            {
                continue;
            }

            sum += building.transform.position;
            count++;
        }

        foreach (abr unit in units)
        {
            if (unit == null)
            {
                continue;
            }

            sum += unit.transform.position;
            count++;
        }

        return count > 0 ? sum / count : Vector3.zero;
    }

    float GetSearchStep()
    {
        MapGenerator mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator != null && mapGenerator.tileSize > 0f)
        {
            return mapGenerator.tileSize;
        }

        return Mathf.Max(1f, slotSearchStep);
    }

    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }
}
