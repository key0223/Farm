using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    /* 언제 도착할지 기록 */
    public void UpdateTimesOnPath()
    {

    }
    public void ClearPath()
    {
        _pathStepStack.Clear();
    }

}
