using UnityEngine;

public class GrassChunk : MonoBehaviour
{
    Renderer rend;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
    }

    public void SetColor(float grassNoiseValue)
    {
        Color color;

        if (grassNoiseValue < 0.33f)
            color = new Color(0.79f, 0.83f, 0.37f);     // mid green
        else if (grassNoiseValue < 0.66f)
            color = new Color(0.48f, 0.73f, 0.29f);     // dark green
        else
            color = new Color(0.89f, 0.80f, 0.66f);     // light green

        rend.material.color = color;
    }
}