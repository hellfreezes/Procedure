using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureGenerator : MonoBehaviour
{
    Mesh currntMesh;
    int spriteWidth = 128;
    int spriteHeight = 128;
    int textureWidth = 1024;
    int textureHeight = 1024;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3[] vert;
        //Vector2[] uv;
        //int[] tri;
        List<Polygon> polyMap = new List<Polygon>();


        currntMesh = GetComponent<MeshFilter>().mesh;

        Polygon poly = new Polygon(0, 0, 0, 1, 0, 1);
        polyMap.Add(poly);
        //uv = GetUV(0, 0);

        currntMesh.Clear();
        currntMesh.vertices = poly.Vertices;
        currntMesh.triangles = poly.Triangles;
        //currntMesh.uv = uv;
        currntMesh.RecalculateBounds();
        currntMesh.RecalculateNormals();
        currntMesh.Optimize();

        GetComponent<MeshCollider>().sharedMesh = currntMesh;
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
                // Do something with the object that was hit by the raycast.

                Debug.Log(hit.triangleIndex);
            }
        }
    }

    Vector2[] GetUV(int u, int v)
    {
        float w = (float)spriteWidth / (float)textureWidth;
        float h = (float)spriteHeight / (float)textureHeight;

        float uN = w * u;
        float vN = h * v;

        Vector2[] result = new Vector2[4];

        result[0] = new Vector2(uN, vN);
        result[1] = new Vector2(uN, vN + h);
        result[2] = new Vector2(uN + w, vN + h);
        result[3] = new Vector2(uN + w, vN);

        return result;
    }
}
