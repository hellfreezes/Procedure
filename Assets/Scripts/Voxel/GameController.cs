using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int blockTall = 256;

    public Texture texture;
    public string texturePath = "";

    public Material worldTexture;
    public GameObject playerPrefab;

    public ColliderController colliderController;

    World world;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("На сцене два GameController. Ошибка.");
        instance = this;
        colliderController = new ColliderController();
        colliderController.CreateGameObjectPool(16, 16, 16);
        //world = new World();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //world.Update();
    }
}
