using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SelectionBox : MonoBehaviour
{
    private Camera cam;
    private Mouse mouse;
    private UnitSelection selectionSystem;

    [Header("UI del Cuadro de Selección")]
    public RectTransform selectionBoxUI;

    private Vector2 startMousePosition;
    private bool isDragging = false;

    private float doubleClickTime = 0.3f;
    private float lastClickTime = -1f;

    void Start()
    {
        cam = Camera.main;
        mouse = Mouse.current;
        selectionSystem = FindFirstObjectByType<UnitSelection>();

        if (selectionBoxUI != null)
        {
            // Centramos el pivote y los anclajes para que concuerden con la matemática local
            selectionBoxUI.pivot = new Vector2(0.5f, 0.5f);
            selectionBoxUI.anchorMin = new Vector2(0.5f, 0.5f);
            selectionBoxUI.anchorMax = new Vector2(0.5f, 0.5f);

            OcultarCuadro();
        }
    }

    void Update()
    {
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            startMousePosition = mouse.position.ReadValue();
            isDragging = true;
        }

        if (isDragging)
        {
            if (mouse.leftButton.isPressed)
            {
                UpdateSelectionBoxUI();
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                FinalizarSeleccion();
            }
        }
    }

    void UpdateSelectionBoxUI()
    {
        if (selectionBoxUI == null) return;

        // 1. Obtenemos el contenedor padre (El Canvas)
        RectTransform parentRect = selectionBoxUI.parent as RectTransform;
        if (parentRect == null) return;

        Vector2 screenStart = startMousePosition;
        Vector2 screenEnd = mouse.position.ReadValue();

        // 2. EL TRADUCTOR MAGICO: Convertimos los píxeles del ratón al tamaño escalado del Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenStart, null, out Vector2 localStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenEnd, null, out Vector2 localEnd);

        // 3. Calculamos el ancho y alto absoluto en unidades de Canvas
        float width = Mathf.Abs(localEnd.x - localStart.x);
        float height = Mathf.Abs(localEnd.y - localStart.y);

        // 4. Calculamos el centro exacto sumando los dos puntos y dividiendo entre dos
        Vector2 center = (localStart + localEnd) / 2f;

        // 5. Aplicamos el tamaño y la posición LOCAL (Esto ignora resoluciones raras)
        selectionBoxUI.sizeDelta = new Vector2(width, height);
        selectionBoxUI.localPosition = center;
    }

    void FinalizarSeleccion()
    {
        isDragging = false;
        OcultarCuadro();

        Vector2 endPos = mouse.position.ReadValue();

        // La selección 3D usa los píxeles reales de la pantalla, así que esto no cambia
        if (Vector2.Distance(startMousePosition, endPos) > 10f)
        {
            if (selectionSystem != null)
            {
                selectionSystem.ClearSelection();
                BuildingInteraction.HideAllUI();

                Vector2 min = Vector2.Min(startMousePosition, endPos);
                Vector2 max = Vector2.Max(startMousePosition, endPos);

                SelectableEntity[] allSelectables = FindObjectsByType<SelectableEntity>(FindObjectsSortMode.None);
                foreach (SelectableEntity selectable in allSelectables)
                {
                    if (selectable == null || !selectable.CanBeSelected || !selectable.AllowBoxSelection) continue;

                    Vector3 screenPos = cam.WorldToScreenPoint(selectable.transform.position);
                    if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                    {
                        selectionSystem.SelectUnit(selectable.gameObject);
                    }
                }
            }
        }
        else
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                SeleccionDobleClic();
            }
            else
            {
                SeleccionIndividual();
            }
            lastClickTime = Time.time;
        }
    }

    void OcultarCuadro()
    {
        if (selectionBoxUI != null)
        {
            selectionBoxUI.sizeDelta = Vector2.zero;
            selectionBoxUI.anchoredPosition = new Vector2(-10000, -10000);
        }
    }

    void SeleccionIndividual()
    {
        if (selectionSystem == null) return;

        selectionSystem.ClearSelection();
        BuildingInteraction.HideAllUI();

        Vector2 mousePos = mouse.position.ReadValue();
        if (SelectionTargetResolver.TryGetTopSelectable(cam, mousePos, out SelectableEntity selectable, out _))
        {
            if (selectable.CanBeSelected)
            {
                selectionSystem.SelectUnit(selectable.gameObject);
                Debug.Log($"<color=white>[CLIC 3D]</color> Seleccionado: {selectable.name}");
                selectable.ShowInspectionUI();
            }
        }
    }

    void SeleccionDobleClic()
    {
        if (selectionSystem == null) return;
        selectionSystem.ClearSelection();
        BuildingInteraction.HideAllUI();

        Vector2 mousePos = mouse.position.ReadValue();
        if (SelectionTargetResolver.TryGetTopSelectable(cam, mousePos, out SelectableEntity clickedSelectable, out _))
        {
            if (clickedSelectable != null)
            {
                SelectableEntity[] allSelectables = FindObjectsByType<SelectableEntity>(FindObjectsSortMode.None);
                foreach (SelectableEntity selectable in allSelectables)
                {
                    if (selectable == null || !selectable.CanBeSelected) continue;
                    if (selectable.Category != clickedSelectable.Category) continue;
                    if (!selectable.AllowBoxSelection) continue;
                    selectionSystem.SelectUnit(selectable.gameObject);
                }
                Debug.Log($"<color=white>[DOBLE CLIC]</color> Seleccionados todos los elementos del mismo tipo.");
            }
        }
    }
}