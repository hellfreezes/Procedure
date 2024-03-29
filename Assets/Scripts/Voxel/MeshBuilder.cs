﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : ThreadedProcess
{
    byte[] faces = new byte[Chunk.size.x * Chunk.size.y * Chunk.size.z];

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    Vector3Int position;
    Block[] blocks;

    int sizeEstimate = 0;
    int vertexIndex = 0, triangleIndex = 0;
    bool isVisible = false;

    public MeshBuilder(Vector3Int pos, Block[] blocks)
    {
        this.position = pos;
        this.blocks = blocks;
    }

    // главная функция генерирующая мешь в многопоточности
    public override void ThreadFunction()
    {
        int index = 0;

        // Поиск соседних чанков
        Chunk[] neighbors = new Chunk[6];
        bool[] exists = new bool[6];

        exists[0] = World.Instance.GetChunkAt(position.x, position.y, position.z + Chunk.size.z, out neighbors[0]);
        exists[1] = World.Instance.GetChunkAt(position.x + Chunk.size.x, position.y, position.z, out neighbors[1]);
        exists[2] = World.Instance.GetChunkAt(position.x, position.y, position.z - Chunk.size.z, out neighbors[2]);
        exists[3] = World.Instance.GetChunkAt(position.x - Chunk.size.x, position.y, position.z, out neighbors[3]);

        exists[4] = World.Instance.GetChunkAt(position.x, position.y + Chunk.size.y, position.z, out neighbors[4]);
        exists[5] = World.Instance.GetChunkAt(position.x, position.y - Chunk.size.y, position.z, out neighbors[5]);

        // Generate faces
        for (int x = 0; x < Chunk.size.x; x++)
        {
            for (int y = 0; y < Chunk.size.y; y++)
            {
                for (int z = 0; z < Chunk.size.z; z++)
                {
                    // Блок - это воздух, не обыгываем. Переход к новому элементу
                    if (blocks[index] == Block.Air)
                    {
                        faces[index] = 0;
                        index++;
                        continue;
                    }

                    //Куда смотрят стороны блока:

                    // На ЮГ
                    if (z == 0 && (exists[2] == false || neighbors[2].GetBlockAt(position.x + x, position.y + y, position.z + z - 1) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.South;
                        sizeEstimate += 4;
                    }
                    else if (z > 0 && blocks[index - 1] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.South;
                        sizeEstimate += 4;
                    }

                    // На СЕВЕР
                    if (z == Chunk.size.z - 1 && (exists[0] == false || neighbors[0].GetBlockAt(position.x + x, position.y + y, position.z + z + 1) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.North;
                        sizeEstimate += 4;
                    } else if (z < Chunk.size.z - 1 && blocks[index + 1] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.North;
                        sizeEstimate += 4;
                    }

                    // ВНИЗ
                    if (y == 0 && (exists[5] == false || neighbors[5].GetBlockAt(position.x + x, position.y + y - 1, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.Down;
                        sizeEstimate += 4;
                    } else if (y > 0 && blocks[index - Chunk.size.z] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.Down;
                        sizeEstimate += 4;
                    }

                    // ВВЕРХ
                    if (y == Chunk.size.y - 1 && (exists[4] == false || neighbors[4].GetBlockAt(position.x + x, position.y + y + 1, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.Up;
                        sizeEstimate += 4;
                    }
                    else if (y < Chunk.size.y - 1 && blocks[index + Chunk.size.z] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.Up;
                        sizeEstimate += 4;
                    }

                    // На ВОСТОК
                    if (x == 0 && (exists[3] == false || neighbors[3].GetBlockAt(position.x + x - 1, position.y + y, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.West;
                        sizeEstimate += 4;
                    } else if (x > 0 && blocks[index - Chunk.size.z * Chunk.size.y] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.West;
                        sizeEstimate += 4;
                    }


                    // На ЗАПАД
                    if (x == Chunk.size.x - 1 && (exists[1] == false || neighbors[1].GetBlockAt(position.x + x + 1, position.y + y, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Direction.East;
                        sizeEstimate += 4;
                    } else if (x < Chunk.size.z - 1 && blocks[index + Chunk.size.z * Chunk.size.y] == Block.Air)
                    {
                        faces[index] |= (byte)Direction.East;
                        sizeEstimate += 4;
                    }

                    isVisible = true;

                    index++;
                }
            }
        }
        if (isVisible == false)
        {
            return;
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
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex;

                        triangles[triangleIndex + 3] = vertexIndex + 1;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures(blocks[index], Direction.North, vertexIndex, uvs);

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                    if ((faces[index] & (byte)Direction.East) != 0)
                    {

                        vertices[vertexIndex] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex + 1;

                        triangles[triangleIndex + 3] = vertexIndex + 2;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Direction.East, vertexIndex, uvs);

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                    if ((faces[index] & (byte)Direction.South) != 0)
                    {

                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);

                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex + 1;

                        triangles[triangleIndex + 3] = vertexIndex + 2;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Direction.South, vertexIndex, uvs);

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                    if ((faces[index] & (byte)Direction.West) != 0)
                    {

                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);

                        triangles[triangleIndex] = vertexIndex + 1;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex;

                        triangles[triangleIndex + 3] = vertexIndex + 1;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures(blocks[index], Direction.West, vertexIndex, uvs);

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                    if ((faces[index] & (byte)Direction.Up) != 0)
                    {

                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex + 1;

                        triangles[triangleIndex + 3] = vertexIndex + 2;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Direction.Up, vertexIndex, uvs);

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                    if ((faces[index] & (byte)Direction.Down) != 0)
                    {

                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);

                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex + 1;

                        triangles[triangleIndex + 3] = vertexIndex + 2;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Direction.Down, vertexIndex, uvs);

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
