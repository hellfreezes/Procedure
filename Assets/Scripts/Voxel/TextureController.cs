using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureController
{
    // Тайлсет карта
    public static Dictionary<string, Vector2[]> textureMap = new Dictionary<string, Vector2[]>();

    public static void Initialize(string texturePath, Texture texture)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(texturePath);

        foreach (Sprite s in sprites)
        {
            Vector2[] uvs = new Vector2[4];
            // uvs - это не пиксельные координаты - это значение в долях принимает от 0.0 до 1.0
            // поэтому надо вычислять долю
            uvs[0] = new Vector2(s.rect.xMin / texture.width, s.rect.yMin / texture.height);
            uvs[1] = new Vector2(s.rect.xMax / texture.width, s.rect.yMin / texture.height);
            uvs[2] = new Vector2(s.rect.xMin / texture.width, s.rect.yMax / texture.height);
            uvs[3] = new Vector2(s.rect.xMax / texture.width, s.rect.yMax / texture.height);


            // TODO: Исключение. Из ресурсов грузятся "файлы" с этими именами. Их не существует. Какой то глюк :(
            if (s.name != "download" && s.name != "error" && s.name != "loading")
                textureMap.Add(s.name, uvs);
        }
    }

    // Создает uv коордианыт текстуры, для соотвествующего блока в нем фейса с учетом направления
    public static bool AddTextures(Block block, Direction direction, int index, Vector2[] uvs)
    {
        // Получаем название блока
        string key = FastGetKey(block, direction);

        Vector2[] text; // сюда выгружаем текстуру ищем по имени блока
        if (textureMap.TryGetValue(key, out text))
        {
            uvs[index] = text[0];
            uvs[index + 1] = text[1];
            uvs[index + 2] = text[2];
            uvs[index + 3] = text[3];

            return true;
        }

        // Если текстура для блока не найдена в тайлсете, то грузим дефолтную-ошибочную
        text = textureMap["default"];
        uvs[index] = text[0];
        uvs[index + 1] = text[1];
        uvs[index + 2] = text[2];
        uvs[index + 3] = text[3];

        return false;
    }

    // Возвращает название блока, точнее грани
    static string FastGetKey(Block block, Direction direction)
    {
        if (block == Block.Stone)
            return "Stone";
        if (block == Block.Dirt)
            return "Dirt";
        if (block == Block.Grass)
        {
            if (direction == Direction.Up)
                return "Grass_Up";
            else if (direction == Direction.Down)
                return "Dirt";
            return "Grass_Side";
        }
        if (block == Block.WoodPlanks)
            return "WoodPlanks";

        return "default";
    }
}
