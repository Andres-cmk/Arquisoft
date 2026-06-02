using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ActionType { Repair, Gather, Chop, Mine }

    [Header("Button Configuration")]
    public ActionType actionType;
    public BuildingState targetBuilding;
    public ResourceNode targetResource;

    private CursorOrders cursorOrders;
    private UnitSelection unitSelection;

    void Start()
    {
        cursorOrders = FindFirstObjectByType<CursorOrders>();
        unitSelection = FindFirstObjectByType<UnitSelection>();
    }

    void OnDisable()
    {
        if (cursorOrders != null) cursorOrders.ClearUICursor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cursorOrders == null) return;

        if (actionType == ActionType.Repair)
            cursorOrders.ForceUICursor(cursorOrders.cursorRepair);
        else if (actionType == ActionType.Gather || actionType == ActionType.Chop || actionType == ActionType.Mine)
            cursorOrders.ForceUICursor(cursorOrders.cursorEmpanada);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cursorOrders != null) cursorOrders.ClearUICursor();
    }

    public void ExecuteAction()
    {
        int selectedCount = unitSelection != null ? unitSelection.selectedUnits.Count : 0;

        if (actionType == ActionType.Repair)
        {
            if (targetBuilding != null)
            {
                targetBuilding.currentState = BuildingState.State.Repairing;
            }
            Debug.Log($"<color=orange>[ACTION]</color> Sending {selectedCount} units to REPAIR.");
        }
        else if (actionType == ActionType.Gather)
        {
            if (targetBuilding != null)
            {
                targetBuilding.currentState = BuildingState.State.Gathering;
            }
            Debug.Log($"<color=yellow>[ACTION]</color> Sending {selectedCount} units to GATHER.");
        }
        else if (actionType == ActionType.Chop)
        {
            string targetName = targetResource != null ? targetResource.name : "resource";
            Debug.Log($"<color=yellow>[ACTION]</color> Sending {selectedCount} units to CHOP at {targetName}.");
        }
        else
        {
            string targetName = targetResource != null ? targetResource.name : "resource";
            Debug.Log($"<color=yellow>[ACTION]</color> Sending {selectedCount} units to MINE at {targetName}.");
        }

        if (cursorOrders != null) cursorOrders.ClearUICursor();

        // Cerramos el panel usando el nuevo sistema centralizado
        BuildingInteraction.HideAllUI();
    }
}