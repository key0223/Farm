using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Movement Costs")]
    [SerializeField] bool _observePenalties = true;
    [Range(0, 20)]
    [SerializeField] int _pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] int _defaultMovementPenalty = 0;

    GridNodes _gridNodes;
    Node _startNode;
    Node _targetNode;

    int _width;
    int _height;
    int _originX;
    int _originY;

    PriorityQueue<Node> _openSet;
    HashSet<Node> _closedSet;

    bool _pathFound = false;
    
    public bool BuildPath(GameLocation location, Vector2Int start, Vector2Int goal, Stack<PathNode> stack)
    {
        
        _pathFound = false;
        if (SetupPathfindingGrid(location, start, goal) && FindShortestPath())
        {
            UpdateNPCStack(location, stack);
            return true;
        }

        return false;
    }

    bool SetupPathfindingGrid(GameLocation location, Vector2Int startPos, Vector2Int goalPos)
    {
        MapData mapData = location.MapData;

        if (MapManager.Instance.GetMapSize(mapData._mapName, out Vector2Int mapSize, out Vector2Int mapOrigin))
        {
            _gridNodes = new GridNodes(mapSize.x, mapSize.y);
            _width = mapSize.x;
            _height = mapSize.y;
            _originX = mapOrigin.x;
            _originY = mapOrigin.y;

            _openSet = new PriorityQueue<Node>();
            _closedSet = new HashSet<Node>();

        }
        else
            return false;

        _startNode = _gridNodes.GetGridNode(startPos.x - mapOrigin.x, startPos.y - mapOrigin.y);
        _targetNode = _gridNodes.GetGridNode(goalPos.x - mapOrigin.x, goalPos.y - mapOrigin.y);

        
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                var tile = mapData.GetTileData(x+mapOrigin.x,y+mapOrigin.y);
                if (tile == null) continue;

                Node node = _gridNodes.GetGridNode(x, y);
                node._isObstacle = !tile.IsPassable;
                if (tile.GetProperty("npcObstacle") == "true") node._isObstacle = true;

                string terrain = tile.GetProperty("terrain") ?? "default";
                node._movementPenalty = terrain switch
                {
                    "path" => _pathMovementPenalty,
                    "grass" => 3,
                    "water" => 10,
                    _ => _defaultMovementPenalty
                };
            }
        }
        return true;
    }
    bool FindShortestPath()
    {
        _openSet.Enqueue(_startNode);

        while (_openSet.Count > 0)
        {
            Node current = _openSet.Dequeue();
            _closedSet.Add(current);

            if (current == _targetNode)
            {
                _pathFound = true;
                break;
            }

            EvaluateNeighbors(current);
        }

        if (_pathFound)
            return true;
        else return false;
    }

    void EvaluateNeighbors(Node current)
    {
        Vector2Int currentNodeGridPos = current._gridPosition;
        Node neighbor;

        for (int i = -1; i <=1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                neighbor = GetNeighbor(currentNodeGridPos.x + i, currentNodeGridPos.y + j);

                if(neighbor != null)
                {
                    int costToNeighbor;

                    if (_observePenalties)
                        costToNeighbor = current._gCost + GetDistance(current, neighbor) + neighbor._movementPenalty;
                    else
                        costToNeighbor = current._gCost + GetDistance(current,neighbor);

                    bool neighborInOpenSet = _openSet.Contains(neighbor);

                    if (costToNeighbor<neighbor._gCost || !neighborInOpenSet)
                    {
                        neighbor._gCost = costToNeighbor;
                        neighbor._hCost = GetDistance(neighbor, _targetNode);

                        neighbor._parentNode = current;

                        if(!neighborInOpenSet)
                            _openSet.Enqueue(neighbor);
                    }

                }

            }
        }
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a._gridPosition.x -  b._gridPosition.x);
        int dstY = Mathf.Abs(a._gridPosition.y - b._gridPosition.y); 
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    Node GetNeighbor(int neighborPosX, int neighborPosY)
    {
        if(neighborPosX >= _width || neighborPosX <0 || neighborPosY >= _height || neighborPosY <0) return null;

        Node neighbor = _gridNodes.GetGridNode(neighborPosX, neighborPosY);
        if (neighbor._isObstacle || _closedSet.Contains(neighbor)) return null;
        else return neighbor;
    }
    void UpdateNPCStack(GameLocation location, Stack<PathNode> stack)
    {
        Node nextNode = _targetNode;
        while (nextNode != null)
        {
            var step = new PathNode
            {
                MapName = location.MapData._mapName,
                TargetGrid = new Vector2Int(nextNode._gridPosition.x+ _originX , nextNode._gridPosition.y+_originY)
            };
            stack.Push(step);
            nextNode = nextNode._parentNode;
        }
    }

}
