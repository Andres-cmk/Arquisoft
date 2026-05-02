using UnityEngine;

/// Guerrero de Combate a Distancia (Ranged)
/// Hereda de Warrior y especializa el combate a larga distancia.

public class Warrior_Distance : Warrior
{
    [Header("Ranged Combat Settings")]

    public float rangedDamageMultiplier = 1f;
    public float projectileSpeed = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    protected override void Start()
    {
        base.Start();

        health = 80f;
        speed = 5f;
        attackPower = 10f;
        attackRange = 10f;
        attackCooldown = 1.5f;
        armor = 3f;
        rangedDamageMultiplier = 1f;
        
    }

    protected override void Update()
    {
        base.Update();
        
        // Lógica específica de guerrero a distancia
    }

    /// Realiza el ataque a distancia
    protected override void PerformAttack()
    {
        if (currentTarget == null) return;

        if (!hasVisionLine(currentTarget.transform)) return;

        if (navMesh.pathPending) return;

        if (navMesh.remainingDistance > attackRange) return;

        Debug.Log($"<color=red>[debug]</color> La condicion de distanci es: {navMesh.remainingDistance > attackRange} y las diastancia es {navMesh.remainingDistance} tiene path,pending. {navMesh.pathPending}");

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
            Debug.LogWarning($"[Proyectil] No hay prefab de proyectil");
        }

        // Reproducir animación de disparo
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Debug.Log($"{name} realizó un ataque a distancia a {currentTarget.name} con {finalDamage} de daño.");
    }

    public void UpgradeProjectileSpeed(float upgradeAmount)
    {
        projectileSpeed += upgradeAmount;
        Debug.Log($"Velocidad de proyectil mejorada a {projectileSpeed}");
    }
}
