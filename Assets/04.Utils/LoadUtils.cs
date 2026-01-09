using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class LoadUtils 
{
    public static Sprite GetAtlasSprite(string directory, string sheetName, int index)
    {
        string atlasPath = $"{directory}/{sheetName}";
        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>(atlasPath);

        if (spriteAtlas == null)
        {
            Debug.LogWarning($"Couldn't find sprite atlas : {sheetName}");
            return null;
        }

        string spriteName = $"{sheetName}_{index}";
        return spriteAtlas.GetSprite(spriteName);
    }

    public static Sprite[] GetAtlasSprites(string directory, string sheetName, int startIndex,int count)
    {
        Sprite[] sprites = new Sprite[count];

        for (int i = 0; i< count; i++)
        {
            int index = startIndex + i ;
            sprites[i] = GetAtlasSprite(directory, sheetName, index);

            if(sprites[i] == null )
            {
                Debug.LogWarning($"Missing sprite: {sheetName}_{index} in {directory}/{sheetName}");
                return new Sprite[0];
            }
        }

        return sprites;
    }
}
