using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    private Camera cam;
    private Mouse mouse;
    private UnitSelection selectionSystem;

    void Start()
    {
        cam = Camera.main;
        mouse = Mouse.current;
        selectionSystem = GetComponent<UnitSelection>();
    }

    void Update()
    {
        if (mouse == null) return;

        if (selectionSystem.selectedUnits.Count > 0)
        {
            if (mouse.rightButton.wasPressedThisFrame)
                EjecutarOrden3D(false);
            else if (mouse.rightButton.isPressed)
                EjecutarOrden3D(true);
        }
    }

    void EjecutarOrden3D(bool clicMantenido)
    {
        Vector2 mousePos = mouse.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        GameObject objetivoPrioritario = null;
        Vector3 puntoDestino = Vector3.zero;
        int prioridadMaxima = -1;

        foreach (var hit in hits)
        {
            int prioridadActual = -1;
            string tagTocado = hit.collider.tag;

            if (tagTocado == "UnidadEnemiga" || tagTocado == "Unidad") prioridadActual = 4;
            else if (tagTocado == "EdificioEnemigo" || tagTocado == "Edificio") prioridadActual = 3;
            else if (tagTocado == "Empanada") prioridadActual = 2;
            else if (tagTocado == "Suelo") prioridadActual = 1;

            if (prioridadActual > prioridadMaxima)
            {
                prioridadMaxima = prioridadActual;
                objetivoPrioritario = hit.collider.gameObject;
                puntoDestino = hit.point;
            }
        }

        if (objetivoPrioritario != null)
        {
            string tagDestino = objetivoPrioritario.tag;

            if (clicMantenido)
            {
                if (tagDestino == "Suelo")
                    Debug.Log($"<color=#00FFCC>[SIGUIENDO]</color> Actualizando ruta hacia {puntoDestino}");
                return;
            }

            if (tagDestino == "UnidadEnemiga")
            {
                Debug.Log($"<color=red>[ATAQUE]</color> Ataque a unidad enemiga: {objetivoPrioritario.name}");
            }
            else if (tagDestino == "EdificioEnemigo")
            {
                Debug.Log($"<color=red>[ATAQUE]</color> Ataque a edificio enemigo: {objetivoPrioritario.name}");
            }
            else if (tagDestino == "Suelo")
            {
                Debug.Log($"<color=cyan>[MOVIMIENTO]</color> Acción de movimiento hacia {puntoDestino}");
            }
            // --- LÓGICA DE BLOQUEO DE EDIFICIO ---
            else if (tagDestino == "Edificio")
            {
                BuildingState state = objetivoPrioritario.GetComponent<BuildingState>();
                BuildingInteraction buildingUI = objetivoPrioritario.GetComponent<BuildingInteraction>();

                // Si el edificio YA NO está en estado Normal (ya le dimos orden)
                if (state != null && state.currentState != BuildingState.State.Normal)
                {
                    Debug.Log($"<color=orange>[AVISO]</color> El edificio ya está en estado: {state.currentState}. Mostrando solo inspección.");
                    if (buildingUI != null) buildingUI.ShowInspectionUI(); // Abre solo la vida
                }
                // Si el edificio está libre
                else
                {
                    Debug.Log($"<color=yellow>[EDIFICIO]</color> Edificio aliado seleccionado para órdenes: {objetivoPrioritario.name}");
                    if (buildingUI != null) buildingUI.ShowCommandUI(); // Abre vida + botones
                }
            }
        }
    }
}