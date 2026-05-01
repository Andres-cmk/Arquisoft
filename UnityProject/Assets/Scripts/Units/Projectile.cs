using UnityEngine;

/// <summary>
/// Script para proyectiles disparados por Warrior_Distance
/// Se destruye al impactar con un objetivo o después de cierto tiempo.
/// </summary>
public class Projectile : MonoBehaviour
{
    private float damage = 0f;
    private Warrior shooter;
    private float lifetime = 10f;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        // Destruir proyectil si ha pasado demasiado tiempo
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // No impactar con el disparador
        if (collision.gameObject == shooter.gameObject)
        {
            return;
        }

        // Aplicar daño a guerreros enemigos
        Warrior targetWarrior = collision.GetComponent<Warrior>();
        if (targetWarrior != null && targetWarrior != shooter)
        {
            targetWarrior.TakeDamage(damage);
            Debug.Log($"Proyectil impactó a {targetWarrior.name} con {damage} de daño.");
        }

        // Destruir proyectil al impactar
        Destroy(gameObject);
    }

    /// <summary>
    /// Configura el daño del proyectil
    /// </summary>
    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    /// <summary>
    /// Configura quién disparó el proyectil
    /// </summary>
    public void SetShooter(Warrior shooterUnit)
    {
        shooter = shooterUnit;
    }

    /// <summary>
    /// Configura la duración máxima del proyectil
    /// </summary>
    public void SetLifetime(float seconds)
    {
        lifetime = seconds;
    }
}
