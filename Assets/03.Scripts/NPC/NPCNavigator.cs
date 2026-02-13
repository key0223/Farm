using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
/// <summary>
/// NPC 스케쥴 기반 경로 생성
/// </summary>
public class NPCNavigator : MonoBehaviour
{
    NPCMovement _movement;
    Stack<PathNode> _pathStepStack = new Stack<PathNode>();

    public Stack<PathNode> PathStepStack {  get { return _pathStepStack; } }
    void Awake()
    {
        _movement = GetComponent<NPCMovement>();
    }
    
    public void BuildPath(ScheduleData data)
    {
        ClearPath();

        if(data.Location == _movement.CurrentLocation)
        {
            Vector2Int currentCellPos = new Vector2Int(_movement.CurrentCellPos.x, _movement.CurrentCellPos.y);
            Vector2Int targetCellPos = new Vector2Int(data.TargetX, data.TargetY);

            GameLocation location = MapManager.Instance.GetLocation(_movement.CurrentLocation);
            if (location == null) return;

            if(NPCManager.Instance.BuildPath(location,currentCellPos,targetCellPos,_pathStepStack))
            {
                UpdateTimesOnPath();
                _pathStepStack.Pop(); // 시작점 제거
            }
        }
        else if(data.Location != _movement.CurrentLocation)
        {
            MapRoute mapRoute;
            mapRoute = NPCManager.Instance.GetMapRoute(_movement.CurrentLocation , data.Location);

            if(mapRoute != null)
            {
                for (int i = mapRoute.MapPaths.Count - 1; i >= 0; i--)
                {
                    int toCellX, toCellY, fromCellX, fromCellY;

                    MapPath mapPath = mapRoute.MapPaths[i];

                    // 마지막 목적지 인가
                    if (mapPath.ToX >= MAX_GRID_WIDTH || mapPath.ToY >= MAX_GRID_HEIGHT)
                    {
                        toCellX = data.TargetX;
                        toCellY = data.TargetY;
                    }
                    else
                    {
                        toCellX = mapPath.ToX;
                        toCellY = mapPath.ToY;
                    }

                    // 출발지 인가?

                    if (mapPath.FromX >= MAX_GRID_WIDTH || mapPath.FromY >= MAX_GRID_HEIGHT)
                    {
                        fromCellX = _movement.CurrentCellPos.x;
                        fromCellY = _movement.CurrentCellPos.y;
                    }
                    else
                    {
                        fromCellX = mapPath.FromX;
                        fromCellY = mapPath.FromY;
                    }

                    Vector2Int fromCellPos = new Vector2Int(fromCellX, fromCellY);
                    Vector2Int toCellPos = new Vector2Int(toCellX, toCellY);

                    GameLocation location = MapManager.Instance.GetLocation(mapPath.MapName);
                    NPCManager.Instance.BuildPath(location, fromCellPos, toCellPos, _pathStepStack);

                }
            }
        }

        if(_pathStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            _pathStepStack.Pop();

            _movement.SetScheduleDataDetails(data);
        }
    }

    /* 언제 도착할지 기록 */
    public void UpdateTimesOnPath()
    {
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        PathNode prevNPCPathNode = null;

        foreach(PathNode pathNode in _pathStepStack)
        {
            if(prevNPCPathNode == null )
                prevNPCPathNode = pathNode;

            pathNode.Hour = currentGameTime.Hours;
            pathNode.Minute = currentGameTime.Minutes;
            pathNode.Second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;
            if (MovementIsDiagonal(pathNode, prevNPCPathNode))
                movementTimeStep = new TimeSpan(0, 0, (int)(CELL_DIAGONAL_SIZE / SECONDS_PER_GAME_SECOND / _movement.MoveSpeed));
            else
                movementTimeStep = new TimeSpan(0, 0, (int)(CELL_SIZE / SECONDS_PER_GAME_SECOND / _movement.MoveSpeed));
                
            currentGameTime = currentGameTime.Add(movementTimeStep);
            prevNPCPathNode = pathNode;
        }
    }

    bool MovementIsDiagonal(PathNode pathNode, PathNode prevPathNode)
    {
        if ((pathNode.TargetGrid.x != prevPathNode.TargetGrid.x) && (pathNode.TargetGrid.y != prevPathNode.TargetGrid.y))
            return true;
        else 
            return false;
    }
    public void ClearPath()
    {
        _pathStepStack.Clear();
    }

}
