using System.Collections;
using UnityEngine;
using TMPro;

public class EdificioCentral : MonoBehaviour
{
    [Header("Producción")]
    public GameObject unidadPrefab; // Arrastra tu prefab de cuadro verde aquí
    public float tiempoCreacion = 5f;
    public bool estaProduciendo = false;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoProgreso;

    public void IniciarProduccion()
    {
        if (!estaProduciendo)
        {
            StartCoroutine(CrearUnidadRoutine());
        }
    }

    private IEnumerator CrearUnidadRoutine()
    {
        estaProduciendo = true;
        float tiempoRestante = tiempoCreacion;

        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (textoProgreso != null)
                textoProgreso.text = $"Creando unidad: {tiempoRestante:F1}s";
            yield return null;
        }

        // Crear la unidad justo encima del edificio (Z = -1)
        Instantiate(unidadPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.identity);

        estaProduciendo = false;
        if (textoProgreso != null) textoProgreso.text = "Listo";
        Debug.Log("<color=yellow>[EDIFICIO]</color> Unidad creada en Edificio de Ingeniería.");
    }
}