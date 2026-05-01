using UnityEngine;

/// <summary>
/// Guerrero de Combate a Distancia (Ranged)
/// Hereda de Warrior y especializa el combate a larga distancia.
/// </summary>
public class Warrior_Distance : Warrior
{
    [Header("Ranged Combat Settings")]
    public float rangedDamageMultiplier = 1.2f;
    public float projectileSpeed = 20f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float maxRange = 15f;

    protected override void Start()
    {
        // Valores por defecto para Warrior Distance
        health = 80f;
        speed = 5f;
        attackPower = 25f;
        attackRange = 10f;
        attackCooldown = 1.5f;
        armor = 3f;
        
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        // Lógica específica de guerrero a distancia
    }

    /// <summary>
    /// Realiza el ataque a distancia
    /// </summary>
    protected override void PerformAttack()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        // Verificar que el objetivo esté dentro del rango máximo
        if (distanceToTarget > maxRange)
        {
            currentTarget = null;
            return;
        }

        float finalDamage = attackPower * rangedDamageMultiplier;

        // Crear y disparar proyectil
        if (projectilePrefab != null)
        {
            Vector3 shootFrom = shootPoint != null ? shootPoint.position : transform.position;
            GameObject projectile = Instantiate(projectilePrefab, shootFrom, Quaternion.identity);
            
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                Vector3 direction = (currentTarget.transform.position - shootFrom).normalized;
                projectileRb.linearVelocity = direction * projectileSpeed;
            }

            // Pasar información de daño al proyectil
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(finalDamage);
                projectileScript.SetShooter(this);
            }
        }
        else
        {
            // Si no hay prefab de proyectil, aplicar daño directamente
            Warrior targetWarrior = currentTarget.GetComponent<Warrior>();
            if (targetWarrior != null)
            {
                targetWarrior.TakeDamage(finalDamage);
            }
        }

        // Reproducir animación de disparo
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Debug.Log($"{name} realizó un ataque a distancia a {currentTarget.name} con {finalDamage} de daño.");
    }

    /// <summary>
    /// Mejora el rango del guerrero a distancia
    /// </summary>
    public void UpgradeRange(float upgradeAmount)
    {
        maxRange += upgradeAmount;
        attackRange = maxRange * 0.8f;
        Debug.Log($"Rango mejorado a {maxRange}");
    }

    /// <summary>
    /// Mejora la velocidad de proyectil
    /// </summary>
    public void UpgradeProjectileSpeed(float upgradeAmount)
    {
        projectileSpeed += upgradeAmount;
        Debug.Log($"Velocidad de proyectil mejorada a {projectileSpeed}");
    }
}
