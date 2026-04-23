using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class NPCMovement : MonoBehaviour
{
    [Header("NPC Movement Settings")]
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _minSpeed = 1f;
    [SerializeField] float _maxSpeed = 3f;

    NPCController _npcController;
    NPCNavigator _nav;
    Grid _grid;

    string _currentLocation;
    string _targetLocation;

    Vector3Int _currentCellPos;
    Vector3Int _targetCellPos;
    Vector3 _targetWorldPos;

    int _currentDirection;
    int _lastHorizontalDirection;
    int _facingDirectionAtDestination;
    string _targetAnimation;
    string _currentAnimation;

    string _previousPathNodeLocation;
    Vector3Int _nextCellPos;
    Vector3 _nextWorldPos;
    Vector2 _moveDir; // 실제 이동 방향

    bool _isMoving = false;
    bool _npcInitialized;
    bool _sceneLoaded = false;

    Coroutine _coMoveToCellPos;
    WaitForFixedUpdate _waitForFixedUpdate;

    public float MoveSpeed { get { return _moveSpeed; } }
    public string CurrentLocation { get { return _currentLocation; } set { _currentLocation = value; } }
    public Vector3Int CurrentCellPos { get { return _currentCellPos; } }
    public int CurrentDirection { get { return _currentDirection; } set { _currentDirection = value; } }

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
        _npcController = GetComponent<NPCController>();
        _nav = GetComponent<NPCNavigator>();

        _targetLocation = _currentLocation;
        _targetCellPos = _currentCellPos;
        _targetWorldPos = transform.position;

        Debug.Log($"Start Cell {_currentCellPos} ");
    }

    void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoad;
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoad;

        GameSceneManager.Instance.OnBeforeSceneUnload -= BeforeSceneUnloaded;
        GameSceneManager.Instance.OnBeforeSceneUnload += BeforeSceneUnloaded;

    }
    void OnDisable()
    {

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoad;
        GameSceneManager.Instance.OnBeforeSceneUnload -= BeforeSceneUnloaded;

    }
    void SubscribeEvent()
    {
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoad;
        GameSceneManager.Instance.OnBeforeSceneUnload += BeforeSceneUnloaded;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void Init()
    {
        if (_currentLocation == SceneManager.GetActiveScene().name)
            _npcController.SetNPCActiveInScene();
        else
            _npcController.SetNPCInactiveInScene();

        _previousPathNodeLocation = _currentLocation;
        _nextCellPos = _currentCellPos;

        _targetCellPos = _currentCellPos;
        _targetWorldPos = GetWorldPosition(_targetCellPos);

        _nextWorldPos = GetWorldPosition(_currentCellPos);

        Debug.Log($"Init Cell {_currentCellPos} ");
    }

    public void SetScheduleDataDetails(ScheduleData scheduleData)
    {
        _targetLocation = scheduleData.Location;
        _targetCellPos = new Vector3Int(scheduleData.TargetX, scheduleData.TargetY, 0);
        _targetWorldPos = GridUtils.GridToWorldCenter(_targetCellPos);
        _facingDirectionAtDestination = scheduleData.Facing;
        _targetAnimation = scheduleData.Animation;
        _npcController.NPCAnim.ResetActionState();
        //_currentAnimation = "";
    }
    void FixedUpdate()
    {
        if (!_sceneLoaded) return;

        if (!_isMoving)
        {
            _currentCellPos = GetGridPosition(transform.position);
            _nextCellPos = _currentCellPos;

            if (_nav.PathStepStack.Count > 0)
            {
                PathNode pathNode = _nav.PathStepStack.Peek();
                _currentLocation = pathNode.MapName;

                if (_currentLocation != _previousPathNodeLocation)
                {
                    _currentCellPos = (Vector3Int)pathNode.TargetGrid;
                    _nextCellPos = _currentCellPos;
                    transform.position = GetWorldPosition(_currentCellPos);
                    _previousPathNodeLocation = _currentLocation;
                    _nav.UpdateTimesOnPath();
                }

                if (_currentLocation == SceneManager.GetActiveScene().name)
                {
                    _npcController.SetNPCActiveInScene();

                    pathNode = _nav.PathStepStack.Pop();
                    _nextCellPos = (Vector3Int)pathNode.TargetGrid;

                    TimeSpan pathNodeTime = new TimeSpan(pathNode.Hour, pathNode.Minute, pathNode.Second);
                    MoveToCellPos(_nextCellPos, pathNodeTime, TimeManager.Instance.GetGameTime());
                }
                else
                {
                    _npcController.SetNPCInactiveInScene();

                    _currentCellPos = (Vector3Int)pathNode.TargetGrid;
                    _nextCellPos = _currentCellPos;
                    transform.position = GetWorldPosition(_currentCellPos);

                    TimeSpan pathNodeTime = new TimeSpan(pathNode.Hour, pathNode.Minute, pathNode.Second);
                    TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                    if (pathNodeTime < gameTime)
                    {
                        pathNode = _nav.PathStepStack.Pop();
                        _currentCellPos = (Vector3Int)pathNode.TargetGrid;
                        _nextCellPos = _currentCellPos;
                        transform.position = GetWorldPosition(_currentCellPos);
                    }
                }
            }
            else
            {
                _currentDirection = _facingDirectionAtDestination;

                if (!string.IsNullOrEmpty(_targetAnimation))
                {
                    if (_targetAnimation == "idle")
                    {
                        _npcController.NPCAnim.SetMovementState(false, _currentDirection);
                        //_currentAnimation = _targetAnimation;
                    }
                    else if (_targetAnimation != _currentAnimation)
                    {
                        _npcController.NPCAnim.PlayAction(_targetAnimation, _currentDirection);
                    }

                    _currentAnimation = _targetAnimation;
                }

            }
        }
    }

    void MoveToCellPos(Vector3Int cellPos, TimeSpan pathNodeTime, TimeSpan gameTime)
    {
        _coMoveToCellPos = StartCoroutine(CoMoveToCellPos(cellPos, pathNodeTime, gameTime));
    }

    IEnumerator CoMoveToCellPos(Vector3Int cellPos, TimeSpan pathNodeTime, TimeSpan gameTime)
    {
        _isMoving = true;
        _nextWorldPos = GetWorldPosition(cellPos);

        if (pathNodeTime > gameTime)
        {
            float timeToMove = (float)(pathNodeTime.TotalSeconds - gameTime.TotalSeconds);
            float calculatedSpeed = Mathf.Max(_minSpeed, Vector3.Distance(transform.position, _nextWorldPos) / timeToMove / Define.SECONDS_PER_GAME_SECOND);

            if (calculatedSpeed <= _maxSpeed)
            {
                while (Vector3.Distance(transform.position, _nextWorldPos) > Define.PIXEL_SIZE)
                {
                    Vector3 unitVector = Vector3.Normalize(_nextWorldPos - transform.position);
                    _moveDir = new Vector2(unitVector.x, unitVector.y);

                    if (Mathf.Abs(_moveDir.x) > Mathf.Abs(_moveDir.y))
                    {
                        _currentDirection = _moveDir.x > 0 ? 1 : 0;
                        _lastHorizontalDirection = _currentDirection;
                    }
                    else
                        _currentDirection = _lastHorizontalDirection;

                    Vector2 move = new Vector2(unitVector.x * calculatedSpeed * Time.fixedDeltaTime, unitVector.y * calculatedSpeed * Time.fixedDeltaTime);
                    transform.position += (Vector3)move;

                    _npcController.NPCAnim.SetMovementState(_isMoving, _currentDirection);
                    yield return _waitForFixedUpdate;
                }
            }
        }

        transform.position = _nextWorldPos;
        _currentCellPos = cellPos;
        _nextCellPos = _currentCellPos;
        _isMoving = false;
        _npcController.NPCAnim.SetMovementState(_isMoving, _currentDirection);
    }

    public int GetDirectionIndex(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? 1 : 0;  // 1:Right, 0:Left
        }
        else
        {
            return dir.y > 0 ? 2 : 3;  // 2:Up, 3:Down
        }
    }
    void AfterSceneLoad()
    {
        _grid = FindObjectOfType<Grid>();
        if (!_npcInitialized)
        {
            Init();
            _npcInitialized = true;
        }

        _sceneLoaded = true;
    }

    void BeforeSceneUnloaded()
    {
        _sceneLoaded = false;
    }

    Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (_grid != null)
        {
            return _grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = _grid.CellToWorld(gridPosition);

        return new Vector3(worldPosition.x + CELL_SIZE / 2f, worldPosition.y + CELL_SIZE / 2f, worldPosition.z);
    }

}
