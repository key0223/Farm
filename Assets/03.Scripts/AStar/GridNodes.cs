using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes 
{
    int _nextId = 0;
    int _width;
    int _height;

    Node[,] _gridNode;

    public GridNodes(int _width, int _height)
    {
        this._width = _width;
        this._height = _height;

        _gridNode = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _gridNode[x, y] = new Node(new Vector2Int(x, y),_nextId++);
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < _width && yPosition < _height)
        {
            return _gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }

}
