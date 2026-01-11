using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerMove _playerMove;
    PlayerInventory _playerInven;
    PlayerAnimator _playerAnim;
    PlayerActionHanlder _playerActionHanlder;
    ItemPickup _itemPickup;

    public PlayerMove PlayerMove {  get { return _playerMove; } }
    public PlayerInventory PlayerInven {  get { return _playerInven; } }
    public PlayerAnimator PlayerAnim { get { return _playerAnim; } }
    public PlayerActionHanlder PlayerActionHanlder { get { return _playerActionHanlder; } }
    public ItemPickup PlayerItemPickup {  get { return _itemPickup; } }

    #region Properties

    public bool CanMove {  get { return _playerMove.CanMove; }set { _playerMove.CanMove = value; } }
    public int CurrentDirection 
    { 
        get { return _playerMove.CurrentDirection; }
        set 
        { 
            _playerMove.CurrentDirection = value;
            if (value == 0 || value == 1)  // Left/Right
            {
                _playerMove.LastHorizontalDirection = value;
            }
            _playerAnim.AnimatedSprite.SetDirection(value);
        }
    }
    public Vector3Int CellPos {  get { return _playerMove.CellPos; } set { _playerMove.CellPos = value; } }
    #endregion

    void Awake()
    {
        CacheComponents();
    }

    void CacheComponents()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInven = GetComponent<PlayerInventory>();
        _playerAnim = GetComponent<PlayerAnimator>();
        _playerActionHanlder = GetComponent<PlayerActionHanlder>();
        _itemPickup = GetComponent<ItemPickup>();
    }
 
    public int GetDirectionToMouse(Vector2 mousePos)
    {
        Vector3Int playerCell = CellPos;
        Vector3Int mouseCell = GridUtils.ScreenToGridPos(mousePos);
        
        Vector3Int direction = mouseCell - playerCell;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            return direction.y > 0 ? 2 : 3;
        else
            return direction.x >0 ? 1 : 0;
    }

}
