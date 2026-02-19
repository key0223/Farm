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
    [SerializeField] string _npcStartLocation;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    NPCMovement _npcMovement;
    NPCNavigator _npcNavigator;
    NPCAnimator _npcAnimator;

    bool _npcActiveInScene = false;



    public string NPCName { get { return _npcName; }}
    public NPCMovement NPCMovement { get { return _npcMovement; }}
    public NPCAnimator NPCAnim { get { return _npcAnimator; }}

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _npcMovement = GetComponent<NPCMovement>();
        _npcNavigator = GetComponent<NPCNavigator>();
        _npcAnimator = GetComponent<NPCAnimator>();

        _npcMovement.CurrentLocation = _npcStartLocation;
    }

    public void MoveTo(ScheduleData scheduleData)
    {
        _npcNavigator.BuildPath(scheduleData);
    }

    public void Interact(PlayerController player, Item gift)
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        int faceDir = _npcMovement.GetDirectionIndex(dir);
        _npcMovement.CurrentDirection = faceDir;
        _npcAnimator.SetMovementState(false, faceDir);

        if (gift != null && gift.CanBeGivenAsGift)
        {
            // ReceiveGift(
        }
        else
            StartDialogue(player);

    }

    void StartDialogue(PlayerController player)
    {
        Debug.Log("Interaction Test");
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
