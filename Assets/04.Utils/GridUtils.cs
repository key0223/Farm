using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GridUtils 
{
    public static string GetTileKey(int gridX, int gridY)
    {
        string key = "x" + gridX + "y" + gridY;

        return key;
    }
    public static Vector2Int TileKeyToVector2Int(string key)
    {
        Match match = Regex.Match(key, @"x(-?\d+)y(-?\d+)");
        if (!match.Success)
            return Vector2Int.zero;

        int x = int.Parse(match.Groups[1].Value);
        int y = int.Parse(match.Groups[2].Value);

        return new Vector2Int(x, y);
    }
    public static Vector3Int GetCellPosFromKey(string key)
    {
        if(string.IsNullOrEmpty(key) || !key.StartsWith("x") || !key.StartsWith("y"))
            return Vector3Int.zero;

        try
        {
            int yStart = key.IndexOf('y');
            if (yStart <= 1) return Vector3Int.zero;

            string xStr = key.Substring(1, yStart - 1); 
            string yStr = key.Substring(yStart + 1);

            int x = int.Parse(xStr);
            int y = int.Parse(yStr);
            return new Vector3Int(x, y, 0);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Invalid key format '{key}': {e.Message}");
            return Vector3Int.zero;
        }
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
