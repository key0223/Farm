using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class NPCMovement : MonoBehaviour
{
    [Header("NPC Movement Settings")]
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _minSpeed = 1f;
    [SerializeField] float _maxSpeed = 3f;

    Grid _grid;
    NPCController _npcController;
    NPCNavigator _nav;

    string _currentLocation;
    string _targetLocation;

    Vector3Int _currentCellPos;
    Vector3Int _targetCellPos;
    Vector3 _targetWorldPos;

    int _facingDirectionAtDestination;
    string _targetAnimation;

    string _previousPathNodeLocation;
    Vector3Int _nextCellPos;
    Vector3 _nextWorldPos;
    Vector2 _moveDir; // 실제 이동 방향

    bool _isMoving = false;
    bool _sceneLoaded = false;

    Coroutine _coMoveToCellPos;
    WaitForFixedUpdate _waitForFixedUpdate;

    public float MoveSpeed { get { return _moveSpeed; } }
    public string CurrentLocation {  get { return _currentLocation; } }
    public Vector3Int CurrentCellPos { get { return _currentCellPos; }}

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
        _npcController = GetComponent<NPCController>();
        _nav = GetComponent<NPCNavigator>();

        _targetLocation = _currentLocation;
        _targetCellPos = _currentCellPos;
        _targetWorldPos = transform.position;
    }

    void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        //GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        //GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoaded;

        //GameSceneManager.Instance.OnBeforeSceneUnload -= BeforeSceneUnloaded;
        //GameSceneManager.Instance.OnBeforeSceneUnload += BeforeSceneUnloaded;

    }
    void OnDisable()
    {

        //GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        //GameSceneManager.Instance.OnBeforeSceneUnload -= BeforeSceneUnloaded;

    }
    void SubscribeEvent()
    {
        //GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoaded;
        //GameSceneManager.Instance.OnBeforeSceneUnload += BeforeSceneUnloaded;
        //GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void Init()
    {
        if (_currentLocation == SceneManager.GetActiveScene().name)
            _npcController.SetNPCActiveInScene();
        else
            _npcController.SetNPCInactiveInScene();

        _previousPathNodeLocation = _currentLocation;
        _currentCellPos = GridUtils.WorldToGrid(transform.position);
        _nextCellPos = _currentCellPos;
        _targetCellPos = _currentCellPos;
        _targetWorldPos = GridUtils.GridToWorldCenter(_currentCellPos);
    }

    public void SetScheduleDataDetails(ScheduleData scheduleData)
    {
        _targetLocation = scheduleData.Location;
        _targetCellPos = new Vector3Int(scheduleData.TargetX, scheduleData.TargetY, 0);
        _targetWorldPos = GridUtils.GridToWorldCenter(_targetCellPos);
        _facingDirectionAtDestination = scheduleData.Facing;
        _targetAnimation = scheduleData.Animation;
    }
    void FixedUpdate()
    {
        if (!_sceneLoaded) return;

        if(!_isMoving)
        {
            _currentCellPos = GridUtils.WorldToGrid(transform.position);
            _nextCellPos = _currentCellPos;

            if(_nav.PathStepStack.Count>0)
            {
                PathNode pathNode = _nav.PathStepStack.Peek();
                _currentLocation = pathNode.MapName;

                if(_currentLocation != _previousPathNodeLocation)
                {
                    _currentCellPos = (Vector3Int)pathNode.TargetGrid;
                    _nextCellPos = _currentCellPos;
                    transform.position = GridUtils.GridToWorldCenter(_currentCellPos);
                    _previousPathNodeLocation = _currentLocation;
                    _nav.UpdateTimesOnPath();
                }

                if(_currentLocation == SceneManager.GetActiveScene().name)
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
                    transform.position = GridUtils.GridToWorldCenter(_currentCellPos);

                    TimeSpan pathNodeTime = new TimeSpan(pathNode.Hour,pathNode.Minute, pathNode.Second);
                    TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                    if(pathNodeTime < gameTime)
                    {
                        pathNode = _nav.PathStepStack.Pop();
                        _currentCellPos = (Vector3Int)pathNode.TargetGrid;
                        _nextCellPos = _currentCellPos;
                        transform.position = GridUtils.GridToWorldCenter(_currentCellPos);
                    }
                }
            }
            else if (_targetCellPos != _currentCellPos)
            {
                Vector2Int start = new Vector2Int(_currentCellPos.x, _currentCellPos.y);
                Vector2Int goal = new Vector2Int(_targetCellPos.x, _targetCellPos.y);

                GameLocation location = MapManager.Instance.GetLocation(_currentLocation);
                if (location == null) return;

                /* A* 경로 생성 */
                if (NPCManager.Instance.BuildPath(location, start, goal, _nav.PathStepStack))
                {
                    _nav.UpdateTimesOnPath();
                }
                else
                {
                    /* 경로 없음 : 직선 이동 */
                    _nextCellPos = _targetCellPos;
                    MoveToCellPos(_nextCellPos, TimeManager.Instance.GetGameTime(), TimeManager.Instance.GetGameTime());
                    return;
                }
            }
            else
            {
                //SetNPCFacingDirection();
                //SetNPCEventAnimation();
            }
        }
    }

    void MoveToCellPos(Vector3Int cellPos, TimeSpan pathNodeTime, TimeSpan gameTime)
    {
        _coMoveToCellPos = StartCoroutine(CoMoveToCellPos(cellPos, pathNodeTime, gameTime));
    }

    IEnumerator CoMoveToCellPos(Vector3Int cellPos,TimeSpan pathNodeTime, TimeSpan gameTime)
    {
        _isMoving = true;
        _nextWorldPos = GridUtils.GridToWorldCenter(cellPos);

        if(pathNodeTime> gameTime)
        {
            float timeToMove = (float)(pathNodeTime.TotalSeconds - gameTime.TotalSeconds);
            float calculatedSpeed = Mathf.Max(_minSpeed, Vector3.Distance(transform.position, _nextWorldPos) / timeToMove / Define.SECONDS_PER_GAME_SECOND);

            if(calculatedSpeed <= _maxSpeed)
            {
                while(Vector3.Distance(transform.position,_nextWorldPos)> Define.PIXEL_SIZE)
                {
                    Vector3 unitVector = Vector3.Normalize(_nextWorldPos - transform.position);
                    Vector2 move = new Vector2(unitVector.x * calculatedSpeed * Time.fixedDeltaTime, unitVector.y * calculatedSpeed * Time.fixedDeltaTime);

                    transform.position += (Vector3)move;
                    yield return _waitForFixedUpdate;
                }
            }
        }

        transform.position = _nextWorldPos;
        _currentCellPos = cellPos;
        _nextCellPos = _currentCellPos;
        _isMoving = false;

    }
}
