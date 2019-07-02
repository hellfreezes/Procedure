using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static World instance;


    public static Matrix4x4 id = Matrix4x4.identity;
    public Texture texture;
    public Material material;

    // Хранит все чанки в связи с их позициями в мире
    Dictionary<Vector3Int, Chunk> chunkPosMap;

    public int radius = 2;
    public int height = 8;

    public static World Instance { get => instance; }

    private void Awake()
    {
        TextureController.Initialize("", texture);
        chunkPosMap = new Dictionary<Vector3Int, Chunk>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("Два мира на сцене!!!");
        }
        instance = this;

        // Создаем чанки. В пределах заданных размеров
        for (int x = -radius; x < radius + 1; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = -radius; z < radius+1; z++)
                {
                    Chunk chunk = new Chunk(new Vector3Int(x * Chunk.size.x, y * Chunk.size.y, z * Chunk.size.z));
                    chunk.GenerateBlockArray();

                    chunkPosMap.Add(chunk.position, chunk);
                }
            }
        }

        foreach (Chunk ch in chunkPosMap.Values)
        {
            StartCoroutine(ch.GenerateMesh());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    public bool GetChunkAt(int x, int y, int z, out Chunk chunk)
    {
        Vector3Int key = WorldCoordsToChunkCoords(x, y, z);

        return chunkPosMap.TryGetValue(key, out chunk);
    }

    public static Vector3Int WorldCoordsToChunkCoords(int x, int y, int z)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x / (float)Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt(y / (float)Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt(z / (float)Chunk.size.z) * Chunk.size.z);
    }

    public void Draw()
    {
        foreach (Chunk ch in chunkPosMap.Values)
        {
            if (ch.ready)
            {
                Graphics.DrawMesh(ch.mesh, id, material, 0);
            }
        }
    }
}
