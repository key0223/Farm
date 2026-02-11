using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    AStar _aStar;
    protected override void Awake()
    {
        base.Awake();
        _aStar = GetComponent<AStar>();
        GameManager.Instance.ManagerReady("NPCManager");
    }

    public bool BuildPath(GameLocation gameLocation,Vector2Int start, Vector2Int goal, Stack<PathNode> stack)
    {
        if (_aStar.BuildPath(gameLocation, start, goal, stack))
            return true;
        else return false;
    }
  
}
