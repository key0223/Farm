using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    PlayerController _playerController;
    AnimatedSprite _animatedSprite;
    string _lastAnimState = "";
    bool _isInAction = false;

    public AnimatedSprite AnimatedSprite {  get { return _animatedSprite; } }
    public bool IsInAction { get { return _isInAction; } }
    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _animatedSprite = GetComponent<AnimatedSprite>();
        _animatedSprite.OnAnimationFinished += OnAnimationFinished;
    }

    void Start()
    {
        _animatedSprite.PlayAnimAllLayers("idle");
    }

    public void SetMovementState(bool isMoving,int direction)
    {
        if (_isInAction) return;
      
        string targetAnim = isMoving ? "run" : "idle";

        if (_lastAnimState != targetAnim)
        {
            _animatedSprite.PlayAnimAllLayers(targetAnim);
            _lastAnimState = targetAnim;
        }
        _animatedSprite.SetDirection(direction);
    }

    public void PlayAction(string actionName, int direction)
    {
        _isInAction = true;
        _animatedSprite.SetDirection(direction);
        _animatedSprite.PlayAnimAllLayers(actionName);
        _lastAnimState= actionName;
    }

    void OnAnimationFinished(string actionName, int direction)
    {
        if(actionName != "Idle" && actionName != "run")
        {
            _isInAction = false;
            _playerController.CanMove = true;
        }
    }
}
