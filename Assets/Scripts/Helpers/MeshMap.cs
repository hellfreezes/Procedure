using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMap : MonoBehaviour
{
    private List<Polygon> polygons;
    private int verticesCount = 0;
    private int trianglesCount = 0;

    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        polygons = new List<Polygon>();
        mesh = GetComponent<MeshFilter>().mesh;

        AddPolygon(0, 0, 0, 1, 0, 1);
        AddPolygon(1, 0, 0, 1, 0, 1);

        // Обновляет мэш
        UpdateMesh();
    }

    void AddPolygon(float x, float y, float z, float sizeX, float sizeY, float sizeZ)
    {
        Polygon p = new Polygon(x, y, z, sizeX, sizeY, sizeZ);
        polygons.Add(p);
    }

    //void UpdateMesh(Polygon p)
    //{
    //    Vector3[] newVertices = new Vector3[mesh.vertices.Length + p.Vertices.Length];
    //    Debug.Log("Добавлено вершин: " + newVertices.Length);
    //    int[] newTris = new int[mesh.triangles.Length + p.Triangles.Length];
    //    Debug.Log("Добавлено треугольников: " + newTris.Length);

    //    int vertCounter = 0;
               
    //    // Добавить имеющиеся вершины в список
    //    for (int i = 0; i < mesh.vertices.Length; i++)
    //    {
    //        newVertices[i] = mesh.vertices[i];
    //        vertCounter++;
    //    }

    //    // Добавить треугольники которые были в список
    //    for (int i = 0; i < mesh.triangles.Length; i++)
    //    {
    //        newTris[i] = mesh.triangles[i];
    //    }

    //    // Добавить новые вершины
    //    for (int i = 0; i < p.Vertices.Length; i++)
    //    {
    //        newVertices[i + mesh.vertices.Length] = p.Vertices[i];
    //    }

    //    // Добавить новые треугольники
    //    for (int i = 0; i < p.Triangles.Length; i++)
    //    {
    //        newTris[i + mesh.triangles.Length] = p.Triangles[i]+ vertCounter;
    //    }

    //    // заменить новыми данными данные меша
    //    mesh.Clear();
    //    mesh.vertices = newVertices;
    //    mesh.triangles = newTris;

    //    mesh.RecalculateBounds();
    //    mesh.RecalculateNormals();
    //    mesh.Optimize();
    //}

    void UpdateMesh()
    {
        UpdateCount();

        Vector3[] newVertices = new Vector3[verticesCount];
        Debug.Log("Добавлено вершин: " + newVertices.Length);
        int[] newTris = new int[trianglesCount];
        Debug.Log("Добавлено треугольников: " + newTris.Length);

        int vertIndex = 0;
        int triIndex = 0;
        int currentIndex = 0;
        int currentVertIndex = 0;

        int triangleCounter = 0;
        int j = 0;
        int k = 0;

        foreach (Polygon p in polygons)
        {
            k = 0;
            currentIndex = vertIndex;
            currentVertIndex = currentIndex;
            for (int i = 0; i < p.Vertices.Length; i++)
            {
                newVertices[currentIndex + i] = p.Vertices[i];
                vertIndex++;
            }

            currentIndex = triIndex;
            for (int i = 0; i < p.Triangles.Length; i++)
            {
                newTris[currentIndex + i] = p.Triangles[i] + currentVertIndex;
                triIndex++;
                j++;
                if (j == 3)
                {
                    p.TriangleIndex[k] = triangleCounter;

                    k++;
                    j = 0;
                    triangleCounter++;
                }
            }
        }

        // заменить новыми данными данные меша
        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = newTris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
    }

    void UpdateCount()
    {
        verticesCount = 0;
        trianglesCount = 0;
        foreach(Polygon p in polygons)
        {
            verticesCount += p.Vertices.Length;
            trianglesCount += p.Triangles.Length;
        }
    }

    Polygon FindByTriangle(int index)
    {
        foreach (Polygon p in polygons)
        {
            if (p.TriangleIndex[0] == index || p.TriangleIndex[1] == index)
            {
                return p;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
