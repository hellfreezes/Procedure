using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController
{
    public ColliderController()
    {

    }

    public void CreateGameObjectPool (int x, int y, int z)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    GameObject go = new GameObject();
                    go.AddComponent<BoxCollider>();
                }
            }
        }
    }
}
