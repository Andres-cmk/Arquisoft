using UnityEngine;

public class RtsNetworkEntity : MonoBehaviour
{
    [SerializeField] int entityId;
    [SerializeField] int ownerSlot = -1;
    [SerializeField] RtsEntityKind kind = RtsEntityKind.None;

    public int EntityId => entityId;
    public int OwnerSlot => ownerSlot;
    public RtsEntityKind Kind => kind;

    void OnEnable()
    {
        RtsEntityRegistry.Register(this);
        RefreshLocalCategory();
    }

    void OnDisable()
    {
        RtsEntityRegistry.Unregister(this);
    }

    public void Configure(int newEntityId, int newOwnerSlot, RtsEntityKind newKind)
    {
        if (entityId != 0)
        {
            RtsEntityRegistry.Unregister(this);
        }

        entityId = newEntityId;
        ownerSlot = newOwnerSlot;
        kind = newKind;

        RtsEntityRegistry.Register(this);
        RefreshLocalCategory();
    }

    public bool IsOwnedByLocalPlayer()
    {
        int localSlot = MultiplayerBootstrap.Instance != null ? MultiplayerBootstrap.Instance.GetLocalPlayerSlot() : 0;
        return ownerSlot >= 0 && ownerSlot == localSlot;
    }

    public void RefreshLocalCategory()
    {
        SelectableEntity selectable = GetComponent<SelectableEntity>();
        if (selectable == null)
        {
            selectable = GetComponentInChildren<SelectableEntity>();
        }

        if (selectable == null)
        {
            return;
        }

        switch (kind)
        {
            case RtsEntityKind.Unit:
                selectable.SetRuntimeCategory(IsOwnedByLocalPlayer()
                    ? SelectableEntity.SelectableCategory.Unit
                    : SelectableEntity.SelectableCategory.EnemyUnit);
                selectable.SetRuntimeBoxSelection(IsOwnedByLocalPlayer());
                break;
            case RtsEntityKind.Building:
                selectable.SetRuntimeCategory(IsOwnedByLocalPlayer()
                    ? SelectableEntity.SelectableCategory.Building
                    : SelectableEntity.SelectableCategory.EnemyBuilding);
                selectable.SetRuntimeBoxSelection(false);
                break;
            case RtsEntityKind.Resource:
                selectable.SetRuntimeCategory(SelectableEntity.SelectableCategory.Resource);
                selectable.SetRuntimeBoxSelection(false);
                break;
        }
    }
}
