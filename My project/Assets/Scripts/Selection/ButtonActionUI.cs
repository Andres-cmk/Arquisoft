using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ActionType { Repair, Gather }

    [Header("Button Configuration")]
    public ActionType actionType;
    public BuildingState targetBuilding;

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
        else if (actionType == ActionType.Gather)
            cursorOrders.ForceUICursor(cursorOrders.cursorEmpanada);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cursorOrders != null) cursorOrders.ClearUICursor();
    }

    public void ExecuteAction()
    {
        if (actionType == ActionType.Repair)
        {
            targetBuilding.currentState = BuildingState.State.Repairing;
            Debug.Log($"<color=orange>[ACTION]</color> Sending {unitSelection.selectedUnits.Count} units to REPAIR.");
        }
        else
        {
            targetBuilding.currentState = BuildingState.State.Gathering;
            Debug.Log($"<color=yellow>[ACTION]</color> Sending {unitSelection.selectedUnits.Count} units to GATHER.");
        }

        if (cursorOrders != null) cursorOrders.ClearUICursor();

        // Cerramos el panel usando el nuevo sistema centralizado
        BuildingInteraction.HideAllUI();
    }
}