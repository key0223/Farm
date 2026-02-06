using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NPCMovement : MonoBehaviour
{
    SceneName _currentScene;
    SceneName _targetScene;

    Vector3Int _currentCellPos;
    Vector3Int _targetCellPos;
    Vector3 _targetWorldPos;

    int _facingDirectionAtDestination;

    SceneName _previousPathNodeScene;
    Vector2Int _nextCellPos;
    Vector2Int _nextWorldPos;
    Vector2 _moveDir; // 실제 이동 방향

    [Header("NPC Movement Settings")]
    [SerializeField] float _moveSpeed = 2f;

    bool _isMovingToNext = false;

    Grid _grid;
    BoxCollider2D _boxCollider;


}
