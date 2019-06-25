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

        GetComponent<MeshCollider>().sharedMesh = mesh;
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

    void Extrude(Polygon p)
    {
        AddPolygon(p.Position.x, p.Position.y + 1, p.Position.z, 1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                Debug.Log(objectHit);

                Polygon p = FindByTriangle(hit.triangleIndex);
                if (p != null)
                {
                    Extrude(p);
                    UpdateMesh();
                }
            }
        }
    }
}
