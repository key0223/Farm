using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int _gridPosition;
    public readonly int _id;
    public int _gCost = 0; // distance from starting node
    public int _hCost = 0; // distance from finishing node
    public bool _isObstacle = false;
    public int _movementPenalty; // 타일별 이동 비용
    public Node _parentNode;

    public int FCost
    {
        get { return _gCost + _hCost; }
    }

    public Node(Vector2Int pos, int uniqueId)
    {
        _gridPosition = pos;
        _id = uniqueId;
        _parentNode = null;
    }

    public int CompareTo(Node other)
    {
        int fCompare = FCost.CompareTo(other.FCost);

        return fCompare != 0 ? fCompare : _hCost.CompareTo(other._hCost);
    }

    public override bool Equals(object obj)
    {
        return obj is Node n && _id == n._id;
    }
    public override int GetHashCode()
    {
        return _id;
    }
}
