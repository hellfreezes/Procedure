using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tests : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    Vector3[] vert;
    Vector2[] uvs;
    int[] tri;

    // Start is called before the first frame update
    void Start()
    {
        vert = new Vector3[4];
        vert[0] = new Vector3(0, 0, 1);
        vert[1] = new Vector3(0, 1, 1);
        vert[2] = new Vector3(1, 0, 1);
        vert[3] = new Vector3(1, 1, 1);
        

        tri = new int[] { 2, 3, 1, 2, 1, 0 };

        uvs = new Vector2[4];

        mesh = new Mesh
        {
            vertices = vert,
            uv = uvs,
            triangles = tri
        };

        mesh.RecalculateNormals();
    }

    public void Update()
    {
        // will make the mesh appear in the Scene at origin position
        Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
    }
}
