using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static Vector3Int size = new Vector3Int(16, 32, 16);
    public Mesh mesh;
    public Vector3Int position;
    public bool ready = false;

    Block[] blocks;

    public Chunk(Vector3Int pos)
    {
        position = pos;
    }

    public void GenerateBlockArray()
    {
        blocks = new Block[size.x * size.y * size.z];
        int index = 0;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    int value = Mathf.CeilToInt(Mathf.PerlinNoise((x + position.x) / 32f, (z + position.z) / 32f) * 15f + 84f);

                    if (y + position.y > value)
                    {
                        index++;
                        continue;
                    }

                    if (y + position.y == value)
                    {
                        blocks[index] = Block.Grass;
                    }

                    if (y + position.y < value && y + position.y > value - 3)
                    {
                        blocks[index] = Block.Dirt;
                    }

                    if (y + position.y <= value - 3)
                    {
                        blocks[index] = Block.Stone;
                    }
                    index++;
                }
            }
        }
    }

    public IEnumerator GenerateMesh()
    {
        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        ready = true;
        builder = null;
    }

    public Block GetBlockAt(int x, int y, int z)
    {
        x -= position.x;
        y -= position.y;
        z -= position.z;

        if (IsPointWithinBounds(x, y, z))
            return blocks[x * Chunk.size.y * Chunk.size.z + y * Chunk.size.z + z]; // Формула вычисления индекса блока из координат xyz
                                                                                   // Зависит от порядка в тройном цикле создания блоков в чанке в GenerateBlockArray()

        return Block.Air;
    }

    /// <summary>
    /// Находится ли блок в пределах координат исходя из размера чанка
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    bool IsPointWithinBounds(int x, int y, int z)
    {
        return x >= 0 && y >= 0 && z >= 0 && z < Chunk.size.z && y < Chunk.size.y && x < Chunk.size.x;
    }
}
