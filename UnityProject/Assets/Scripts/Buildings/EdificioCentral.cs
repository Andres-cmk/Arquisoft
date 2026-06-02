using System.Collections;
using UnityEngine;
using TMPro;

public class EdificioCentral : MonoBehaviour
{
    [Header("Producción")]
    public GameObject unidadPrefab; // Arrastra tu prefab de cuadro verde aquí
    public float tiempoCreacion = 5f;
    public bool estaProduciendo = false;
    public Transform puntoSpawn;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoProgreso;

    private BuildingState buildingState;
    private Coroutine networkProductionRoutine;

    void Awake()
    {
        buildingState = GetComponent<BuildingState>();
    }

    public void IniciarProduccion()
    {
        if (RtsNetworkCommandBus.TryRequestProduction(this))
        {
            return;
        }

        if (!estaProduciendo)
        {
            StartCoroutine(CrearUnidadRoutine());
        }
        else
        {
            Debug.Log("<color=orange>[EDIFICIO]</color> Este edificio ya está generando una unidad.");
        }
    }

    private IEnumerator CrearUnidadRoutine()
    {
        estaProduciendo = true;
        if (buildingState != null)
        {
            buildingState.currentState = BuildingState.State.Generating;
        }

        float tiempoRestante = tiempoCreacion;

        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (textoProgreso != null)
                textoProgreso.text = $"Creando unidad: {tiempoRestante:F1}s";
            yield return null;
        }

        if (unidadPrefab == null)
        {
            Debug.LogWarning("[EDIFICIO] No hay unidadPrefab asignado en EdificioCentral.");
            estaProduciendo = false;
            if (buildingState != null && buildingState.currentState == BuildingState.State.Generating)
            {
                buildingState.currentState = BuildingState.State.Normal;
            }
            yield break;
        }

        Vector3 spawnPosition = puntoSpawn != null ? puntoSpawn.position : transform.position + new Vector3(0, 1.6f, 10);
        Quaternion spawnRotation = puntoSpawn != null ? puntoSpawn.rotation : unidadPrefab.transform.rotation;

        Instantiate(unidadPrefab, spawnPosition, spawnRotation);

        estaProduciendo = false;
        if (buildingState != null && buildingState.currentState == BuildingState.State.Generating)
        {
            buildingState.currentState = BuildingState.State.Normal;
        }

        if (textoProgreso != null) textoProgreso.text = "Listo";
        Debug.Log("<color=yellow>[EDIFICIO]</color> Unidad creada en Edificio de Ingeniería.");
    }

    public float GetProductionDuration()
    {
        return Mathf.Max(0f, tiempoCreacion);
    }

    public Vector3 GetSpawnPosition()
    {
        return puntoSpawn != null ? puntoSpawn.position : transform.position + new Vector3(0, 1.6f, 10);
    }

    public Quaternion GetSpawnRotation()
    {
        return puntoSpawn != null && unidadPrefab != null ? puntoSpawn.rotation : (unidadPrefab != null ? unidadPrefab.transform.rotation : transform.rotation);
    }

    public RtsUnitType GetProducedUnitType()
    {
        return RtsUnitTypeUtility.GetUnitType(unidadPrefab);
    }

    public void BeginNetworkProductionVisual()
    {
        if (networkProductionRoutine != null)
        {
            StopCoroutine(networkProductionRoutine);
        }

        networkProductionRoutine = StartCoroutine(NetworkProductionVisualRoutine());
    }

    private IEnumerator NetworkProductionVisualRoutine()
    {
        estaProduciendo = true;
        if (buildingState != null)
        {
            buildingState.currentState = BuildingState.State.Generating;
        }

        float tiempoRestante = tiempoCreacion;
        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (textoProgreso != null)
            {
                textoProgreso.text = $"Creando unidad: {tiempoRestante:F1}s";
            }
            yield return null;
        }

        if (textoProgreso != null)
        {
            textoProgreso.text = "Esperando host";
        }
    }

    public GameObject SpawnProducedUnitFromNetwork(
        int entityId,
        int ownerSlot,
        Vector3 spawnPosition,
        Quaternion spawnRotation,
        RtsUnitType expectedUnitType = RtsUnitType.Unknown)
    {
        if (RtsEntityRegistry.TryGetEntity(entityId, out RtsNetworkEntity existingUnit))
        {
            ResetProductionState();
            return existingUnit.gameObject;
        }

        if (networkProductionRoutine != null)
        {
            StopCoroutine(networkProductionRoutine);
            networkProductionRoutine = null;
        }

        if (unidadPrefab == null)
        {
            Debug.LogWarning("[EDIFICIO] No hay unidadPrefab asignado en EdificioCentral.");
            ResetProductionState();
            return null;
        }

        ValidateProducedUnitType(expectedUnitType);

        GameObject unit = Instantiate(unidadPrefab, spawnPosition, spawnRotation);
        RtsEntityRegistry.GetOrAdd(unit, entityId, ownerSlot, RtsEntityKind.Unit);

        ResetProductionState();
        Debug.Log("<color=yellow>[EDIFICIO]</color> Unidad creada por host multiplayer.");
        return unit;
    }

    void ValidateProducedUnitType(RtsUnitType expectedUnitType)
    {
        if (expectedUnitType == RtsUnitType.Unknown)
        {
            return;
        }

        RtsUnitType localUnitType = GetProducedUnitType();
        if (localUnitType == RtsUnitType.Unknown || localUnitType == expectedUnitType)
        {
            return;
        }

        Debug.LogError(
            "[MULTIPLAYER] Tipo de unidad producida desincronizado. Host espera "
            + RtsUnitTypeUtility.GetDisplayName(expectedUnitType)
            + " pero este edificio produciria "
            + RtsUnitTypeUtility.GetDisplayName(localUnitType)
            + ".");
    }

    private void ResetProductionState()
    {
        estaProduciendo = false;
        if (buildingState != null && buildingState.currentState == BuildingState.State.Generating)
        {
            buildingState.currentState = BuildingState.State.Normal;
        }

        if (textoProgreso != null)
        {
            textoProgreso.text = "Listo";
        }
    }
}
