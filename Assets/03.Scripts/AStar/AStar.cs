using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UI.Image;

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
    int _gridWidth;
    int _gridHeight;

    PriorityQueue<Node> _openSet;
    HashSet<int> _openSetIds;
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
        _gridWidth = mapData._mapWidth;
        _gridHeight = mapData._mapHeight;

        _gridNodes = new GridNodes(mapData);
        _openSet = new PriorityQueue<Node>();
        _openSetIds = new HashSet<int>();
        _closedSet = new HashSet<Node>();

        _startNode = _gridNodes.GetGridNode(startPos.x , startPos.y);
        _targetNode = _gridNodes.GetGridNode(goalPos.x, goalPos.y);

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                var tile = mapData.GetTileData(x, y);
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
        _openSetIds.Add(_startNode._id);

        while (_openSet.Count > 0)
        {
            Node current = _openSet.Dequeue();
            _openSetIds.Remove(current._id);

            if (current == _targetNode)
            {
                _pathFound = true;
                return true;
            }
            if (_closedSet.Contains(current)) continue;
            _closedSet.Add(current);

            EvaluateNeighbors(current);
        }
        return false;
    }

    void EvaluateNeighbors(Node current)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Node neighbor = _gridNodes.GetGridNode(current._gridPosition.x + dx, current._gridPosition.y + dy);
                if (neighbor == null || neighbor._isObstacle || _closedSet.Contains(neighbor)) continue;

                int distance = GetDistance(current._gridPosition, neighbor._gridPosition);
                int newGCost = current._gCost + distance + (_observePenalties ? neighbor._movementPenalty : 0);

                if (!_openSetIds.Contains(neighbor._id) || newGCost < neighbor._gCost)
                {
                    if (_openSetIds.Contains(neighbor._id)) _openSetIds.Remove(neighbor._id);
                    neighbor._gCost = newGCost;
                    neighbor._hCost = GetDistance(neighbor._gridPosition, _targetNode._gridPosition);
                    neighbor._parentNode = current;
                    _openSet.Enqueue(neighbor);
                    _openSetIds.Add(neighbor._id);
                }
            }
        }
    }

    int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dstX = Mathf.Abs(a.x - b.x), dstY = Mathf.Abs(a.y - b.y);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    void UpdateNPCStack(GameLocation location, Stack<PathNode> stack)
    {
        Node node = _targetNode;
        while (node != null)
        {
            var step = new PathNode
            {
                MapName = location.MapData._mapName,
                TargetGrid = new Vector2Int(node._gridPosition.x , node._gridPosition.y )
            };
            stack.Push(step);
            node = node._parentNode;
        }
    }

}
