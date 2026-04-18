using UnityEngine;

public class FogVisibility : MonoBehaviour
{
    Renderer rend;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        int x = Mathf.FloorToInt(pos.x / FowManager.Instance.tileSize);
        int y = Mathf.FloorToInt(pos.z / FowManager.Instance.tileSize);

        int state = FowManager.Instance.GetFogState(x, y);

        if (state == 0)
        {
            rend.enabled = false;
        }
        else
        {
            rend.enabled = true;
        }
    }
}