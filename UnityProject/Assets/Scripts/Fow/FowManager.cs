using UnityEngine;
using System.Collections.Generic;

public class FowManager : MonoBehaviour
{
    public static FowManager Instance;

    const int StateUnexplored = 0;
    const int StateExplored = 1;
    const int StateVisible = 2;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int fowWidth = 50;
    public int fowHeight = 50;
    public float tileSize = 7f;

    public Material fowMaterial;
    GameObject fowPlane;

    Texture2D fowTexture;
    int[,] fowState;

    readonly HashSet<int> visibleThisFrame = new HashSet<int>();
    readonly HashSet<int> dirtyPixels = new HashSet<int>();

    void Start()
    {
        initFow();
        createFowPlane();
    }

    void initFow()
    {
        fowTexture = new Texture2D(fowWidth, fowHeight);
        fowTexture.filterMode = FilterMode.Point;
        fowTexture.wrapMode = TextureWrapMode.Clamp;

        fowState = new int[fowWidth, fowHeight];

        for (int x = 0; x < fowWidth; x++)
        {
            for (int y = 0; y < fowHeight; y++)
            {
                fowState[x, y] = StateUnexplored;
                dirtyPixels.Add(x + y * fowWidth);
            }
        }

        UpdateFowTexture();
    }

    public void Reveal(int x, int y, int radius)
    {
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && nx < fowWidth && ny >= 0 && ny < fowHeight)
                {
                    float distance = Mathf.Sqrt(i * i + j * j);

                    if (distance <= radius)
                    {
                        int index = nx + ny * fowWidth;

                        if (fowState[nx, ny] != StateVisible)
                        {
                            fowState[nx, ny] = StateVisible;
                            dirtyPixels.Add(index);
                        }

                        visibleThisFrame.Add(index);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        bool stateChanged = false;

        for (int x = 0; x < fowWidth; x++)
        {
            for (int y = 0; y < fowHeight; y++)
            {
                int index = x + y * fowWidth;

                if (fowState[x, y] == StateVisible && !visibleThisFrame.Contains(index))
                {
                    fowState[x, y] = StateExplored;
                    dirtyPixels.Add(index);
                    stateChanged = true;
                }
            }
        }

        visibleThisFrame.Clear();

        if (stateChanged || dirtyPixels.Count > 0)
        {
            UpdateFowTexture();
        }
    }

    void UpdateFowTexture()
    {
        foreach (int index in dirtyPixels)
        {
            int x = index % fowWidth;
            int y = index / fowWidth;

            Color color;

            if (fowState[x, y] == StateUnexplored)
                color = new Color(0, 0, 0, 1f);
            else if (fowState[x, y] == StateExplored)
                color = new Color(0, 0, 0, 0.7f);
            else
                color = new Color(0, 0, 0, 0f);

            fowTexture.SetPixel(x, y, color);
        }

        fowTexture.Apply();
        dirtyPixels.Clear();

        Shader.SetGlobalTexture("_FowTex", fowTexture);
        Shader.SetGlobalFloat("_FowWidth", fowWidth);
        Shader.SetGlobalFloat("_FowHeight", fowHeight);
        Shader.SetGlobalFloat("_TileSize", tileSize);
    }

    public void RevealWorld(Vector3 worldPos, int radius)
    {
        int x = Mathf.FloorToInt(worldPos.x / tileSize);
        int y = Mathf.FloorToInt(worldPos.z / tileSize);
        Reveal(x, y, radius);
    }

    void createFowPlane()
    {
        fowPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);

        fowPlane.transform.position = new Vector3(
            (fowWidth * tileSize) / 2 - tileSize / 2,
            0.05f,
            (fowHeight * tileSize) / 2 - tileSize / 2
        );

        fowPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        fowPlane.transform.localScale = new Vector3(fowWidth * tileSize, fowHeight * tileSize, 1);

        Renderer rend = fowPlane.GetComponent<Renderer>();

        if (fowMaterial == null)
        {
            Debug.LogError("FowManager: fowMaterial no asignado.");
            return;
        }

        rend.material = fowMaterial;
        rend.material.mainTexture = fowTexture;
    }

    public int GetFogState(int x, int y)
    {
        if (x < 0 || x >= fowWidth || y < 0 || y >= fowHeight)
            return 0;

        return fowState[x, y];
    }
}