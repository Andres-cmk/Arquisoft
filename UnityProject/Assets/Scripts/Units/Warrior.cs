using UnityEngine;

/// Clase base para unidades guerreras.
/// Hereda de Humano y añade funcionalidades de combate comunes.
/// Sirve como clase base para Warrior_Mele y Warrior_Distance.

public abstract class Warrior : Humano
{
    [Header("Warrior Settings")]
    public float attackRange;
    public float attackPower = 25f;
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
        currentTarget = targetHuman != null ? targetHuman.gameObject : null;
        moveTarget = currentTarget != null ? currentTarget.transform.position : target;
        resourceTarget = targetResource;
        resourceActionPending = targetResource != null;

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
        else if(targetHuman != null)
        {
            resourceTarget = null;
            resourceActionPending = false;
            RestartMovement();
            navMesh.SetDestination(moveTarget);
            hasMoveOrder = true;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
            Debug.Log($"Unidad moviendose a {moveTarget} para atacar a {targetHuman.name}.");

        }
        else
        {
            RestartMovement();
            navMesh.SetDestination(moveTarget);
            hasMoveOrder = true;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
            Debug.Log($"Unidad moviendose a {moveTarget}.");
        }
    }

    public void UpdateTargetFromNetWork()
    {
        if(currentTarget != null){
            moveTarget = currentTarget.transform.position;
            float distanceToTarget = GetCurrentTargetDistance();

            if(distanceToTarget > GetEffectiveAttackRange() || !hasVisionLine(currentTarget.transform)){
                RestartMovement();
                navMesh.SetDestination(moveTarget);
                hasMoveOrder = true;
            }
            else{
                StopMovement();
                movement = Vector3.zero;
                hasMoveOrder = false;
                previousDistanceToTarget = -1f;
                stuckTimer = 0f;
            }
        }
    }


    public bool hasVisionLine(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            return false;
        }

        Vector3 origen = transform.position + Vector3.up * 1.5f;
        Vector3 destino = targetTransform.position + Vector3.up * 1.5f;
        Vector3 toTarget = destino - origen;
        float distancia = toTarget.magnitude;

        if (distancia <= 0.01f)
        {
            return true;
        }

        Vector3 direccion = toTarget / distancia;
        RaycastHit[] hits = Physics.RaycastAll(origen, direccion, distancia);
        RaycastHit closestHit = default;
        bool hasClosestHit = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger)
            {
                continue;
            }

            Transform hitTransform = hit.collider.transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform))
            {
                continue;
            }

            if (!hasClosestHit || hit.distance < closestHit.distance)
            {
                closestHit = hit;
                hasClosestHit = true;
            }
        }

        if (!hasClosestHit)
        {
            return true;
        }

        Transform closestTransform = closestHit.collider.transform;
        return closestTransform == targetTransform
            || closestTransform.IsChildOf(targetTransform)
            || targetTransform.IsChildOf(closestTransform);

    }


    /// Método abstracto para el ataque específico de cada tipo de guerrero
    protected abstract void PerformAttack();

    protected virtual void UpdateAttack()
    {
        if (currentTarget == null)
        {
            return;
        }

        float distanceToTarget = GetCurrentTargetDistance();

        // Si el objetivo está fuera del rango, cancelar
        if (distanceToTarget > GetEffectiveAttackRange())
        {
            return;
        }

        if (!hasVisionLine(currentTarget.transform))
        {
            return;
        }

        if (distanceToTarget <= GetEffectiveAttackRange())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
    }

    protected float GetCurrentTargetDistance()
    {
        if (currentTarget == null)
        {
            return float.MaxValue;
        }

        return Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    protected float GetEffectiveAttackRange()
    {
        return attackRange + 2f;
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
