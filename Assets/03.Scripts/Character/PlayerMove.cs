using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerMove : MonoBehaviour
{
    PlayerController _playerController;
    InputState _input;

    float _moveSpeed = 5;
    Vector2 _inputDir; /* 현재 프레임 입력 방향 */
    Vector2 _moveDir; /* 실제 이동 방향 */

    Vector3Int _cellPos; /* 현재 타일 좌표 */

    bool _canMove = true;
    bool _isMoving;
    int _currentDirection; /* 0 : Left, 1 : Right, 2 : Up, 3 : Down */
    int _lastHorizontalDirection = 1;

    public Vector3Int CellPos { get { return _cellPos; } set { _cellPos = value; } }
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public int CurrentDirection { get { return _currentDirection; } set { _currentDirection = value; } }
    public int LastHorizontalDirection { get { return _lastHorizontalDirection; } set {_lastHorizontalDirection = value; } }
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _input = InputManager.Instance.InputState;

        _cellPos = GridUtils.WorldToGrid(transform.position);
    }
    void Update()
    {
        if (InputManager.Instance == null) return;

        if (_playerController.PlayerItemPickup != null)
            _playerController.PlayerItemPickup.CheckAutoPickup(_cellPos);

        if (!_canMove)
            return;

        UpdateDirInput();
        UpdateMoving();
        UpdateDirection();


        /* 애니메이션 상태 업데이트 */
        _playerController.PlayerAnim.SetMovementState(_isMoving, _currentDirection);
    }

    void UpdateDirInput()
    {
        _inputDir = Vector2.zero;

        if (_input.IsKeyHeld(Keys.W))
            _inputDir.y += 1f;
        if (_input.IsKeyHeld(Keys.S))
            _inputDir.y -= 1f;
        if (_input.IsKeyHeld(Keys.D))
            _inputDir.x += 1f;
        if (_input.IsKeyHeld(Keys.A))
            _inputDir.x -= 1f;

        if (_inputDir.sqrMagnitude > 1f)
            _inputDir.Normalize();

        _isMoving = _inputDir.sqrMagnitude > 0.01f;
        if (_isMoving)
            _moveDir = _inputDir.normalized;
    }
    void UpdateMoving()
    {
        if (!_isMoving) return;

        Vector3 targetPos = transform.position + (Vector3)_moveDir * _moveSpeed * Time.deltaTime;

        Vector3Int currentCell = GridUtils.WorldToGrid(transform.position);
        Vector3Int targetCell = GridUtils.WorldToGrid(targetPos);

        if (currentCell != targetCell && !CanGo(targetCell))
            return;

        transform.position = targetPos;
        _cellPos = targetCell;
    }
  
    bool CanGo(Vector3Int targetGrids)
    {
        TileData targetTile = MapManager.Instance.CurrentLocation.MapData.GetTileData(targetGrids.x, targetGrids.y);
        return targetTile == null || targetTile.IsPassable;
    }
    void UpdateDirection()
    {
        if (!_isMoving) return;

        if (Mathf.Abs(_moveDir.x) > Mathf.Abs(_moveDir.y))
        {
            _currentDirection = _moveDir.x > 0 ? 1 : 0;
            _lastHorizontalDirection = _currentDirection;
        }
        else
            _currentDirection = _lastHorizontalDirection;
    }

    public Vector3Int GetCurrentDirection()
    {
        /* 0 : Left, 1 : Right, 2 : Up, 3 : Down */

        return _currentDirection switch
        {
            0 => Vector3Int.left,   // Left
            1 => Vector3Int.right,  // Right
            2 => Vector3Int.up,     // Up
            3 => Vector3Int.down,   // Down
            _ => Vector3Int.zero   // 기본값
        };
    }
}
