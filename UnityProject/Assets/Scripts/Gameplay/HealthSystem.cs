using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth = 1000f;

    public Slider healthBar;

    void Start()
    {
        // Al iniciar el edificio siempre tiene vida completa
        currentHealth = maxHealth;
    }

    public void ActualizarUI()
    {
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

            // Forzar que el área de llenado se estire correctamente
            RectTransform fillRect = healthBar.fillRect;
            if (fillRect != null) fillRect.anchorMax = new Vector2(currentHealth / maxHealth, 1);
        }
    }
}