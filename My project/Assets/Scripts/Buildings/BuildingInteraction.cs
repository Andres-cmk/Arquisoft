using UnityEngine;

public class BuildingInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainPanel;       // Arrastra aquí tu Panel_Info_Edificio
    public GameObject actionButtons;   // Arrastra aquí el nuevo ActionButtons vacío

    // Usamos estático para saber qué edificio tiene la UI abierta y evitar superposiciones
    private static BuildingInteraction activeBuilding;

    // Clic Izquierdo: Solo inspeccionar
    public void ShowInspectionUI()
    {
        if (activeBuilding != null && activeBuilding != this) activeBuilding.HideUI();
        activeBuilding = this;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (actionButtons != null) actionButtons.SetActive(false); // Apagamos botones
    }

    // Clic Derecho: Dar orden
    public void ShowCommandUI()
    {
        if (activeBuilding != null && activeBuilding != this) activeBuilding.HideUI();
        activeBuilding = this;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (actionButtons != null) actionButtons.SetActive(true); // Encendemos botones
    }

    public void HideUI()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (activeBuilding == this) activeBuilding = null;
    }

    // Método global para apagar la UI desde cualquier otro script
    public static void HideAllUI()
    {
        if (activeBuilding != null) activeBuilding.HideUI();
    }
}