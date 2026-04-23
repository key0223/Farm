using UnityEngine;

public class GridNodes 
{
    int _nextId = 0;
    int _width;
    int _height;

    Node[,] _gridNode;

    public GridNodes(int width, int height)
    {

        _width = width;
        _height = height;

        _gridNode =new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridNode[x, y] = CreateNode(x,y);
            }
        }
    }

    Node CreateNode(int x, int y)
    {
        int id = _nextId++;

        return new Node(new Vector2Int(x, y), id);
    }

   
    public Node GetGridNode(int posX, int posY)
    {
        if (posX < _width && posY < _height)
        {
            return _gridNode[posX, posY];
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
