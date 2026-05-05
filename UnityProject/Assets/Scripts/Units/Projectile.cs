using UnityEngine;

/// <summary>
/// Script para proyectiles disparados por Warrior_Distance
/// Se destruye al impactar con un objetivo o después de cierto tiempo.
/// </summary>
public class Projectile : MonoBehaviour
{
    private float damage = 10f;
    private Warrior_Distance shooter;
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

    private void OnCollisionEnter(Collision collision)
    {
        

        Debug.Log($"<color=Red>[DEBUG]</color>Llamada a OnCollisionEnter.");
        // No impactar con el que dispara
        if (collision.gameObject == shooter.gameObject) return;

        // Aplicar daño a enemigos
        Humano targetHuman = collision.gameObject.GetComponent<Humano>();
        if (targetHuman != null && targetHuman != shooter)
        {
            targetHuman.TakeDamage(damage);
            Debug.Log($"Proyectil impactó a {targetHuman.name} con {damage} de daño.");
        }

        // Destruir proyectil al impactar
        Destroy(gameObject);
    }

    /// Configura el daño del proyectil
    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    /// Configura quién disparó el proyectil
    public void SetShooter(Warrior_Distance shooterUnit)
    {
        shooter = shooterUnit;
    }

    /// Configura la duración máxima del proyectil
    public void SetLifetime(float seconds)
    {
        lifetime = seconds;
    }
}
