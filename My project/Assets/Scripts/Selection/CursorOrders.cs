using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CursorOrders : MonoBehaviour
{
    [Header("Cursor Textures")]
    public Texture2D cursorGeneral;
    public Texture2D cursorMovement;
    public Texture2D cursorAttack;
    public Texture2D cursorRepair;
    public Texture2D cursorEmpanada;

    [Header("Settings")]
    public Vector2 hotspot = Vector2.zero;

    private UnitSelection unitSelection;
    private Mouse mouse;

    private Texture2D forcedUICursor = null;

    void Start()
    {
        unitSelection = FindFirstObjectByType<UnitSelection>();
        mouse = Mouse.current;

        if (cursorGeneral != null) Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
    }

    public void ForceUICursor(Texture2D texture) { forcedUICursor = texture; }
    public void ClearUICursor() { forcedUICursor = null; }

    void Update()
    {
        if (mouse == null) return;

        // 1. Si la UI nos obliga a mostrar un cursor especial
        if (forcedUICursor != null)
        {
            Cursor.SetCursor(forcedUICursor, hotspot, CursorMode.ForceSoftware);
            return;
        }

        // 2. Si el ratón está sobre cualquier otra parte de la UI normal
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
            return;
        }

        Vector2 mousePos = mouse.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            string touchedTag = hit.collider.tag;
            bool hasUnitsSelected = (unitSelection != null && unitSelection.selectedUnits.Count > 0);

            if (touchedTag == "UnidadEnemiga" || touchedTag == "EdificioEnemigo")
            {
                if (hasUnitsSelected && !mouse.rightButton.isPressed)
                    Cursor.SetCursor(cursorAttack, hotspot, CursorMode.ForceSoftware);
                else
                    Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
            }
            else if (touchedTag == "Suelo")
            {
                if (hasUnitsSelected && mouse.rightButton.isPressed)
                    Cursor.SetCursor(cursorMovement, hotspot, CursorMode.ForceSoftware);
                else
                    Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
            }
            else if (touchedTag == "Edificio")
            {
                BuildingState state = hit.collider.GetComponent<BuildingState>();

                // Aplicamos la Cinta o la Empanada SOLO si el edificio está en ese estado específico
                if (state != null && state.currentState == BuildingState.State.Repairing)
                    Cursor.SetCursor(cursorRepair, hotspot, CursorMode.ForceSoftware);
                else if (state != null && state.currentState == BuildingState.State.Gathering)
                    Cursor.SetCursor(cursorEmpanada, hotspot, CursorMode.ForceSoftware);
                else
                    Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
            }
        }
        else
        {
            Cursor.SetCursor(cursorGeneral, hotspot, CursorMode.ForceSoftware);
        }
    }
}