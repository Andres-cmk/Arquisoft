using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    public List<GameObject> selectedUnits = new List<GameObject>();

    [Header("Referencias UI Edificio")]
    public GameObject panelInfo;
    public Slider healthBarUI;

    void Start()
    {
        // REQUERIMIENTO: Asegurar que el panel inicie apagado al dar Play
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
        }
    }

    public void SelectUnit(GameObject obj)
    {
        if (!selectedUnits.Contains(obj))
        {
            selectedUnits.Add(obj);

            if (obj.CompareTag("Edificio"))
            {
                panelInfo.SetActive(true);

                HealthSystem hp = obj.GetComponent<HealthSystem>();
                if (hp != null)
                {
                    hp.healthBar = healthBarUI;
                    hp.ActualizarUI();
                }
            }
            else if (!HayEdificiosSeleccionados())
            {
                panelInfo.SetActive(false);
            }

            // MIGRACIÓN 3D: Usamos MeshRenderer
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // Modificamos el color del material principal
                renderer.material.color = obj.CompareTag("Unidad") ? Color.cyan : Color.red;
            }
        }
    }

    private bool HayEdificiosSeleccionados()
    {
        foreach (GameObject obj in selectedUnits)
        {
            if (obj != null && obj.CompareTag("Edificio")) return true;
        }
        return false;
    }

    public void ClearSelection()
    {
        foreach (GameObject obj in selectedUnits)
        {
            if (obj != null)
            {
                // MIGRACIÓN 3D: Usamos MeshRenderer
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    // Volver a colores originales
                    renderer.material.color = obj.CompareTag("Unidad") ? Color.green : new Color(0.5f, 0f, 0.5f);
                }
            }
        }
        selectedUnits.Clear();
        if (panelInfo != null) panelInfo.SetActive(false);
    }
}