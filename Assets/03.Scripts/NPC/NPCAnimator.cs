using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SingleAnimatedSprite))]
public class NPCAnimator : MonoBehaviour
{
    NPCController _npcController;
    SingleAnimatedSprite _animatedSprite;

    string _lastAnimState = "";
    bool _isInAction = false;

    void Awake()
    {
        _npcController = GetComponent<NPCController>();
        _animatedSprite = GetComponent<SingleAnimatedSprite>();
        _animatedSprite.OnAnimationFinished += OnAnimationFinished;
        
    }
    void Start()
    {
        _animatedSprite.InitAnimationDict(_npcController.NPCName);
        _animatedSprite.PlayAnim("idle");
    }
   
    public void SetMovementState(bool isMoving,int direction)
    {
        if (_isInAction) return;

        string targetAnim = isMoving ? "walk" : "idle";
        if (_lastAnimState != targetAnim)
        {
            _animatedSprite.PlayAnim(targetAnim);
            _lastAnimState = targetAnim;
        }
        _animatedSprite.SetDirection(direction);
    }
    public void PlayAction(string actionName, int direction)
    {
        _isInAction = true;
        _animatedSprite.SetDirection(direction);
        _animatedSprite.PlayAnim(actionName);
        _lastAnimState = actionName;
    }
    void OnAnimationFinished(string actionName, int direction)
    {
        if (actionName != "idle" && actionName != "walk")
        {
            _isInAction = false;
            _animatedSprite.PlayAnim("idle");
        }
    }
}
