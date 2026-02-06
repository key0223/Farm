using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC 스케쥴 기반 경로 생성
/// </summary>
public class NPCNavigator : MonoBehaviour
{
    NPCMovement _movement;
    Stack<NPCMovement> _pathStepStack = new Stack<NPCMovement>();

    void Awake()
    {
        _movement = GetComponent<NPCMovement>();
    }
    
    public void BuildPath()
    {

    }
    public void ClearPath()
    {
        _pathStepStack.Clear();
    }

}
