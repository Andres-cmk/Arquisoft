using UnityEngine;

/// <summary>
/// Unidad Villager: Constructor/Recolector
/// Hereda todas las funcionalidades base de Humano.
/// Especializada en recolección de recursos.
/// </summary>
public class Villager : Humano
{
    [Header("Villager Settings")]
    public float harvestSpeed = 1.0f; // Velocidad de recolección de recursos

    protected override void Start()
    {
        // Valores por defecto para Villager
        health = 60f;
        speed = 5f;
        
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        // Lógica específica de Villager aquí
    }


    public override void SetMoveTargetFromNetwork(Vector3 target, ResourceNode targetResource = null, Humano targetHuman = null)
    {
        
        moveTarget = target;
        resourceTarget = targetResource;
        resourceActionPending = targetResource != null;

        if(navMesh == null)
        {
            Debug.LogWarning($"[UNIDAD] {name} no tiene NavMeshAgent. No se puede ejecutar orden de movimiento.");
            return;
        }
        else if(this.GetComponentInChildren<Warrior>() == null && targetHuman != null)
        {
            Debug.Log($"<color=Red>[UNIDAD]</color> {name} no es un Warrior y no puede atacar unidades. Orden de movimiento ignorada.");
            return;
        }
        else
        {
            navMesh.SetDestination(moveTarget);
            hasMoveOrder = true;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
            Debug.Log($"Unidad moviendose a {moveTarget} {(resourceTarget != null ? "para recolectar recurso" : "")}.");
        }
    }


    public override bool CheckArrival()
    {
        if (!navMesh.pathPending && navMesh.remainingDistance <= 2f && resourceTarget != null)
        {
            Debug.Log("Villager está dentro del rango deseado de recolección");
            if (resourceTarget != null && resourceActionPending)
            {
                resourceActionPending = false;
                CompleteResourceAction();

                Debug.Log($"<color=yellow>[UNIDAD]</color> {name} ha llegado a {resourceTarget.name} y comienza a recolectar.");
            }
            return true;
        }
        return false;        
    }

    protected override void CompleteResourceAction()
    {
        if (resourceTarget == null)
        {
            return;
        }

        if (!RtsNetworkCommandBus.TryHandleResourceArrival(this, resourceTarget))
        {
            resourceTarget.FarmResource();
        }
    }    

    /// <summary>
    /// Método para mejorar la velocidad de recolección
    /// </summary>
    public void BoostHarvestSpeed(float multiplier)
    {
        harvestSpeed *= multiplier;
        Debug.Log($"Velocidad de recolección mejorada a {harvestSpeed}");
    }
}
