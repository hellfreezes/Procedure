using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureController
{
    public static Dictionary<string, Vector2[]> textureMap = new Dictionary<string, Vector2[]>();

    public static void Initialize(string texturePath)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(texturePath);
        Texture texture = Resources.Load<Texture>(texturePath);

        Debug.Log(texture.name);
    }

    public static bool AddTextures(Block block, Direction direction, int index, Vector2[] uvs)
    {

    }
}
