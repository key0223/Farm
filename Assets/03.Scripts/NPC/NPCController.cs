using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] string _npcName;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    NPCMovement _npcMovement;
    NPCNavigator _npcNavigator;

    bool _npcActiveInScene = false;



    public string NPCName { get { return _npcName; }}

    void Start()
    {
        _npcMovement = GetComponent<NPCMovement>();
        _npcNavigator = GetComponent<NPCNavigator>();
    }

    public void MoveTo(ScheduleData scheduleData)
    {
        _npcNavigator.BuildPath(scheduleData);
        // ResetAnimation

    }

    public void ResetDay()
    {
        _npcNavigator.ClearPath();
    }
    public void SetNPCActiveInScene()
    {
        _spriteRenderer.enabled = true;
        _boxCollider2D.enabled = true;
        _npcActiveInScene = true;
    }
    
    public void SetNPCInactiveInScene()
    {
        _spriteRenderer.enabled = false;
        _boxCollider2D.enabled = false;
        _npcActiveInScene = false;
    }

}
