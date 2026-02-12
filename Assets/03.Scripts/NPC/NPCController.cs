using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent (typeof(NPCNavigator))]
[RequireComponent(typeof(NPCSchedule))]
[RequireComponent(typeof(NPCAnimator))]
public class NPCController : MonoBehaviour
{
    [SerializeField] string _npcName;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    NPCMovement _npcMovement;
    NPCNavigator _npcNavigator;
    NPCAnimator _npcAnimator;

    bool _npcActiveInScene = false;



    public string NPCName { get { return _npcName; }}
    public NPCAnimator NPCAnim { get { return _npcAnimator; }}

    void Start()
    {
        _npcMovement = GetComponent<NPCMovement>();
        _npcNavigator = GetComponent<NPCNavigator>();
        _npcAnimator = GetComponent<NPCAnimator>();
    }

    public void MoveTo(ScheduleData scheduleData)
    {
        _npcNavigator.BuildPath(scheduleData);
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
