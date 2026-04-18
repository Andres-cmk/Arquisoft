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

    void Awake()
    {
        buildingState = GetComponent<BuildingState>();
    }

    public void IniciarProduccion()
    {
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
}