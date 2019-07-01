using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : ThreadedProcess
{
    byte[] faces = new byte[Chunk.size.x * Chunk.size.y * Chunk.size.x];

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    Vector3Int position;
    Block[] blocks;

    int sizeEstimate = 0;
    int vertexIndex = 0, triangleIndex = 0;
    bool isVisible = true;

    public MeshBuilder(Vector3Int pos, Block[] blocks)
    {
        this.position = pos;
        this.blocks = blocks;
    }

    public override void ThreadFunction()
    {
        int index = 0;

        // Generate faces
        for (int x = 0; x < Chunk.size.x; x++)
        {
            for (int y = 0; y < Chunk.size.y; y++)
            {
                for (int z = 0; z < Chunk.size.z; z++)
                {
                    if (blocks[index].IsTransparent())
                    {
                        faces[index] = 0;
                        index++;
                        continue;
                    }

                    if (z == 0)
                    {
                        faces[index] |= (byte)Direction.South;
                        sizeEstimate += 4;
                    }
                    if (z == Chunk.size.z - 1)
                    {
                        faces[index] |= (byte)Direction.North;
                        sizeEstimate += 4;
                    }
                    if (y == 0)
                    {
                        faces[index] |= (byte)Direction.Down;
                        sizeEstimate += 4;
                    }
                    if (y == Chunk.size.z - 1)
                    {
                        faces[index] |= (byte)Direction.Up;
                        sizeEstimate += 4;
                    }
                    if (x == 0)
                    {
                        faces[index] |= (byte)Direction.West;
                        sizeEstimate += 4;
                    }
                    if (x == Chunk.size.z - 1)
                    {
                        faces[index] |= (byte)Direction.East;
                        sizeEstimate += 4;
                    }

                    index++;
                }
            }
        }

        index = 0;

        // Generate mesh

        vertices = new Vector3[sizeEstimate];
        uvs = new Vector2[sizeEstimate];
        triangles = new int[(int)(sizeEstimate * 1.5)];

        for (int x = 0; x < Chunk.size.x; x++)
        {
            for (int y = 0; y < Chunk.size.y; y++)
            {
                for (int z = 0; z < Chunk.size.z; z++)
                {
                    if (faces[index] == 0)
                    {
                        index++;
                        continue;
                    }

                    if ((faces[index] & (byte)Direction.North) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[triangleIndex] = vertexIndex + 1;
                        triangles[triangleIndex + 1] = vertexIndex + 1;
                        triangles[triangleIndex + 2] = vertexIndex;

                        triangles[triangleIndex + 3] = vertexIndex + 1;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 2;

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }

                    index++;
                }
            }
        }
    }

    public Mesh GetMesh(ref Mesh copy)
    {
        if (copy == null)
        {
            copy = new Mesh();
        } else
        {
            copy.Clear();

        }

        if (isVisible == false || vertexIndex == 0)
        {
            return copy;
        }

        if (vertexIndex > 65000)
            copy.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        copy.vertices = vertices;
        copy.uv = uvs;
        copy.triangles = triangles;

        copy.RecalculateNormals();

        return copy;
    }
}
