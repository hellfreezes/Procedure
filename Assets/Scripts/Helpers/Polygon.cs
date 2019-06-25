using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    public Vector3 Position { get; set; }
    public string Name { get; set; }
    public Vector3[] Vertices { get; set; }
    public int[] Triangles { get; set; }
    public Vector2[] UV { get; set; }

    public int[] TriangleIndex { get; set; }

    public Polygon(float x, float y, float z, float sizeX, float sizeY, float sizeZ)
    {
        TriangleIndex = new int[2];
        Position = new Vector3(x, y, z);

        Name = "[" + x + "," + y + "]";

        Vector3 p1 = new Vector3(x, y, z);
        Vector3 p2 = new Vector3(x, y, z + sizeZ);
        Vector3 p3 = new Vector3(x + sizeX, y, z + sizeZ);
        Vector3 p4 = new Vector3(x + sizeX, y, z);

        Vertices = new Vector3[] { p1, p2, p3, p4 };

        Triangles = new int[] { 0, 1, 2, 0, 2, 3 };

    }
}
