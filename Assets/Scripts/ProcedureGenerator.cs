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
        Vector3[] vert;
        Vector2[] uv;
        int[] tri;

        currntMesh = GetComponent<MeshFilter>().mesh;

        Vector3 p1 = new Vector3(0, 0, 0);
        Vector3 p2 = new Vector3(0, 1, 0);
        Vector3 p3 = new Vector3(1, 1, 0);
        Vector3 p4 = new Vector3(1, 0, 0);

        //Vector3 p5 = new Vector3(0, 0, 1);
        //Vector3 p6 = new Vector3(0, 1, 1);
        //Vector3 p7 = new Vector3(1, 1, 1);
        //Vector3 p8 = new Vector3(1, 0, 1);

        vert = new Vector3[] { p1, p2, p3, p4 };//, p5, p6, p7, p8};



        tri = new int[] { 0, 1, 2, 0, 2, 3 };
                         /*0,5,1, 0,4,5,
                         4,6,5, 4,7,6,
                         3,2,6, 3,6,7,
                         1,5,2, 2,5,6,
                         0,3,4, 3,7,4};*/


        uv = GetUV(0, 0);

        currntMesh.Clear();
        currntMesh.vertices = vert;
        currntMesh.triangles = tri;
        currntMesh.uv = uv;
        currntMesh.RecalculateBounds();
        currntMesh.RecalculateNormals();
        currntMesh.Optimize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2[] GetUV(int u, int v)
    {
        float w = spriteWidth / textureWidth;
        float h = spriteHeight / textureHeight;

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
