using UnityEngine;

/// <summary>
/// Unidad Exploradora
/// Hereda de Humano y se especializa en reconocimiento.
/// Caracterizada por su mayor velocidad de movimiento.
/// </summary>
public class Explorer : Humano
{
    [Header("Explorer Settings")]
    public float visionRange = 25f;
    public float speedMultiplier = 1.4f; 

    protected override void Start()
    {
        health = 50f;
        speed = 7f;
        stoppingDistance = 0.15f;
        
        base.Start();

        // Aplicar multiplicador de velocidad
        if (navMesh != null)
        {
            navMesh.speed = speed * speedMultiplier;
        }
    }

    protected override void Update()
    {
        base.Update();
        
    }


    public void UpgradeSpeed(float upgradeAmount)
    {
        speed += upgradeAmount;
        if (navMesh != null)
        {
            navMesh.speed = speed;
        }
        Debug.Log($"Velocidad mejorada a {speed}");
    }

}
