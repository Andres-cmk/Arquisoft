using UnityEngine;

/// <summary>
/// Clase base para unidades guerreras.
/// Hereda de Humano y añade funcionalidades de combate comunes.
/// Sirve como clase base para Warrior_Mele y Warrior_Distance.
/// </summary>
public abstract class Warrior : Humano
{
    [Header("Warrior Settings")]
    public float attackPower = 25f;
    public float attackRange = 1f;
    public float attackCooldown = 1.0f;
    public float armor = 5f;

    protected float lastAttackTime = -Mathf.Infinity;
    public GameObject currentTarget;

    protected override void Start()
    {
        // Valores por defecto para Warrior
        health = 100f;
        speed = 4.5f;

        currentTarget = null;
        
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (currentTarget != null)
        {
            UpdateTargetFromNetWork();
            UpdateAttack();
        }
    }


    public override void SetMoveTargetFromNetwork(Vector3 target, ResourceNode targetResource = null, Humano targetHuman = null)
    {
        moveTarget = target;
        currentTarget = targetHuman != null ? targetHuman.gameObject : null;
        resourceActionPending = targetResource != null;

        Debug.LogWarning($"[Debug] componente en Villager en hijos {this.GetComponentInChildren<Villager>()} , resource: {targetResource}.");

        if(navMesh == null)
        {
            Debug.LogWarning($"[UNIDAD] {name} no tiene NavMeshAgent. No se puede ejecutar orden de movimiento.");
            return;
        }
        else if(this.GetComponentInChildren<Villager>() == null && targetResource != null)
        {
            Debug.Log($"<color=Red>[UNIDAD]</color> {name} no es un Villager y no puede recolectar recursos. Orden de movimiento ignorada.");
            resourceTarget = null;
            resourceActionPending = false;
            return;
        }
        else if(targetHuman != null){
            navMesh.SetDestination(targetHuman.transform.position);
            hasMoveOrder = true;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
            Debug.Log($"1Unidad moviendose a {moveTarget} {(targetHuman != null ? "para recolectar Atacar" : "")}.");
        }
        else
        {
            navMesh.SetDestination(moveTarget);
            hasMoveOrder = true;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
            Debug.Log($"2Unidad moviendose a {moveTarget} {(targetHuman != null ? "para recolectar Atacar" : "")}.");
        }
    }

    public void UpdateTargetFromNetWork()
    {
        if(currentTarget != null){
            navMesh.SetDestination(currentTarget.transform.position);
            //Debug.Log($"<color=yellow>[DEBUG]</color> Recalculando ruta de {name} {(currentTarget.name != null ? "para recolectar Atacar" : "")}.");
        }
    }

    /// Método abstracto para el ataque específico de cada tipo de guerrero
    protected abstract void PerformAttack();

    protected virtual void UpdateAttack()
    {
        if (currentTarget == null)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        // Si el objetivo está fuera del rango, cancelar
        if (distanceToTarget > attackRange + 2f)
        {
            return;
        }

        if (distanceToTarget <= attackRange + 2f)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Debug.LogWarning($"[Debug] Time debug.");
                //CancelMoveOrder();
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
    }


    public override bool TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(1f, damage - armor);

        Debug.Log($"{name} tiene de salud actual: {this.health}");

        this.health -= actualDamage;

        Debug.Log($"{name} recibió {actualDamage} daño. Salud actual: {this.health}");

        if (this.health <= 0)
        {
            Die();
            return false;
        }
        return true;
    }

    protected override void Die()
    {
        Debug.Log($"{name} ha muerto.");
        Destroy(gameObject);
    }
}
