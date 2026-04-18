using UnityEngine;

public class FowRevealer : MonoBehaviour
{
    public int visionRadius = 3;

    void Update()
    {
        if (FowManager.Instance == null || visionRadius <= 0)
        {
            return;
        }

        FowManager.Instance.RevealWorld(transform.position, visionRadius);
    }
}