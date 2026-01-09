using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils 
{
    public static string GetTileKey(int gridX, int gridY)
    {
        string key = "x" + gridX + "y" + gridY;

        return key;
    }
    public static Vector3Int GetCellPosFromKey(string key)
    {
        int xStart = key.IndexOf('x');
        int yStart = key.IndexOf('y');

        int x = int.Parse(key.Substring(xStart, yStart - xStart));
        int y = int.Parse(key.Substring(yStart));
        return new Vector3Int(x, y, 0);
    }
    /* Å¸ÀÏ ÀÎµ¦½º, ÁÂÇÏ´Ü ±âÁØ */
    public static Vector3Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);

        return new Vector3Int(x, y, 0);
    }

    /* ÁÂÇÏ´Ü ¿ùµå ÁÂÇ¥ */
    public static Vector3 GridToWorld(Vector3Int gridPos)
    {
        float x = gridPos.x * Define.CELL_SIZE;
        float y = gridPos.y * Define.CELL_SIZE;
        return new Vector2(x, y);
    }

    // Å¸ÀÏ Áß¾Ó ¿ùµå ÁÂÇ¥
    public static Vector3 GridToWorldCenter(Vector3Int gridPos)
    {
        float x = (gridPos.x + 0.5f);
        float y = (gridPos.y + 0.5f);
        return new Vector3(x, y, 0f);
    }

    public static Vector3Int ScreenToGridPos(Vector2 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

        Vector3Int tileIndex = WorldToGrid(worldPos);  

        return tileIndex;
    }
}
