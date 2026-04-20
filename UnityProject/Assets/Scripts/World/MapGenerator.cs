using UnityEngine;

using Unity.AI.Navigation;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public GameObject grass_chunk;
    public GameObject forest_chunk;
    public GameObject gold_chunk;
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float tileSize = 7f;
    public float grassNoiseScale = 0.1f;
    public float forestNoiseScale = 0.15f;
    public float goldNoiseScale = 0.05f;
    void Start()
    {
        generateTerrain();
    }

    void generateTerrain()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                float grassNoiseValue = Mathf.PerlinNoise(x * grassNoiseScale, z * grassNoiseScale);
                float forestNoiseValue = Mathf.PerlinNoise(x * forestNoiseScale, z * forestNoiseScale);
                float goldNoiseValue = Mathf.PerlinNoise(x * goldNoiseScale, z * goldNoiseScale);

                Vector3 pos = new Vector3(x * tileSize, 0, z * tileSize);

                GameObject grass = Instantiate(grass_chunk, pos, Quaternion.identity);

                GrassChunk gc = grass.GetComponent<GrassChunk>();
                gc.SetColor(grassNoiseValue);

                if (forestNoiseValue < 0.28f)
                {
                    GameObject forest = Instantiate(forest_chunk, pos, Quaternion.identity);
                } else if (goldNoiseValue < 0.15f)
                {
                    GameObject gold = Instantiate(gold_chunk, pos, Quaternion.identity);
                }                

            }
        }

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}