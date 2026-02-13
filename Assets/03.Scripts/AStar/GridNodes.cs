using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GridNodes 
{
    int _nextId = 0;
    int _width;
    int _height;
    int _minX;
    int _minY;

    Node[,] _gridNode;

    public GridNodes(MapData mapData)
    {
        _nextId = 0;

        _width = mapData._actualWidth;
        _height = mapData._actualHeight;
        _minX = mapData._minX;
        _minY = mapData._minY;

        _gridNode =new Node[_width, _height];

        foreach(var kvp in mapData.Tiles)
        {
            Vector2Int tilePos =GridUtils.TileKeyToVector2Int(kvp.Key);
            int arrayX = tilePos.x - _minX;
            int arrayY = tilePos.y - _minY;

            TileData tile = kvp.Value;

            Node node = CreateNode(tilePos, tile);
            _gridNode[arrayX,arrayY] = node;
            
        }
    }

    Node CreateNode(Vector2Int tilePos,TileData data)
    {
        int id = _nextId++;

        return new Node(tilePos, id)
        {
            _isObstacle = !data.IsPassable,
            //_movementPenalty = GetPenalty(),
        };
    }
    public Node GetGridNode(int posX, int posY)
    {
        int arrayX = posX - _minX;
        int arrayY = posY - _minY;

        if (arrayX < _width && arrayY < _height)
        {
            return _gridNode[arrayX, arrayY];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }

    int GetPenalty(string terrain)
    {
        return terrain switch
        {
            "path" => 0,
            "grass" => 3,
            "water" => 10,
            _ => 1
        };
    }

}
