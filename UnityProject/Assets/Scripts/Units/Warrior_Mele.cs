using UnityEngine;


public class Warrior_Mele : Warrior
{
    [Header("Melee Combat Settings")]
    public float meleeDamageMultiplier = 1.5f;
    public float knockbackForce = 150f;
    public float attackAnimationDuration = 0.5f;

    protected override void Start()
    {
        // Valores por defecto para Warrior Melee
        health = 120f;
        speed = 4.5f;
        attackPower = 10f;
        attackRange = 1.2f;
        attackCooldown = 1.2f;
        armor = 8f;
        
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        // Lógica específica de guerrero melee
    }


    /// Realiza el ataque cuerpo a cuerpo
    protected override void PerformAttack()
    {
        if (currentTarget == null) return;

        float finalDamage = attackPower * meleeDamageMultiplier;
        
        Humano targetUnit = currentTarget.GetComponent<Humano>();
        
        if (targetUnit != null)
        {
            bool targetKeepsAlive = targetUnit.TakeDamage(finalDamage);

            if (!targetKeepsAlive) 
            {
                currentTarget = null;
                return;
            }
        }
        
        // Aplicar knockback
        Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Debug.Log($"Aplicando knockback a {currentTarget.name}");
            Vector3 knockbackDirection = (currentTarget.transform.position - transform.position).normalized;

            Debug.Log($"[DEBUG] Knockback direction: {knockbackDirection}");

            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }

        // Reproducir animación de ataque
        if (anim != null)
        {
            //anim.SetTrigger("Attack");
        }

        Debug.Log($"{name} realizó un ataque melee a {currentTarget.name} con {finalDamage} de daño.");
    }

    public void UpgradeMeleeDamage(float upgradeAmount)
    {
        attackPower += upgradeAmount;
        Debug.Log($"Daño melee mejorado a {attackPower}");
    }
}
