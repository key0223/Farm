using SuperTiled2Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : SingletonMonobehaviour<MapManager>
{
    public event Action<GameLocation> OnLocationChanged;

    [SerializeField] SuperMap[] _superMaps;
    [Header("Tiles")]
    [SerializeField] TileBase _dugTile;
    [SerializeField] TileBase _wateredTile;
    Dictionary<string,GameLocation> _gameLocations = new Dictionary<string, GameLocation>();

    GameLocation _currentLocation;
    Grid _grid;

    Transform _itemsParent;
    public TileBase DugTile { get { return _dugTile; } }
    public TileBase WateredTile { get { return _wateredTile; } }
    public GameLocation CurrentLocation { get { return _currentLocation; } }
    public Transform ItemsParent { get { return _itemsParent; } }

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        RegisterLocations();
        GameManager.Instance.ManagerReady("MapManager");

    }

    void Start()
    {
        Init();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoaded;

        TimeManager.Instance.OnDayPassed -= AdvanceDay;
        TimeManager.Instance.OnDayPassed += AdvanceDay;
    }
    void OnDisable()
    {
        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        TimeManager.Instance.OnDayPassed -= AdvanceDay;

    }

    void SubscribeEvent()
    {
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoaded;
        TimeManager.Instance.OnDayPassed += AdvanceDay;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void RegisterLocations()
    {
       foreach (SuperMap superMap in _superMaps)
        {
            string mapName = superMap.gameObject.name;
            if (_gameLocations.ContainsKey(mapName)) return;

            GameLocation location = new GameLocation(superMap);
            _gameLocations.Add(mapName, location);

            //if(_currentLocation == null)
            //{
            //    _currentLocation = location;
            //    _grid = location.Grid;
            //    OnLocationChanged?.Invoke(location);
            //}
        }
    }

    void Init()
    {
        string startSceneName = GetMapName(GameSceneManager.Instance.StartScene.ToString());

        foreach (var kvp in _gameLocations)
        {
            string mapName = kvp.Key;

            if(startSceneName == mapName)
            {
                _currentLocation = kvp.Value;
                OnLocationChanged?.Invoke(kvp.Value);
            }
        }
    }
    public void SetCurrentLocation(string sceneName)
    {
        string mapName = GetMapName(sceneName);

        if (_gameLocations.TryGetValue(mapName, out GameLocation location))
        {
            _currentLocation = location;
            OnLocationChanged?.Invoke(location);
        }

    }

    public GameLocation GetLocation(string name)
    {
        _gameLocations.TryGetValue(name,out GameLocation location);
        return location;
    }

    void AdvanceDay()
    {
        foreach (GameLocation location in _gameLocations.Values)
        {
            location.DayUpdateAll();
        }
    }

    string GetMapName(string sceneName)
    {
        int index = sceneName.IndexOf('_');
        string mapName = sceneName.Substring(index + 1);

        return mapName;
    }

    void AfterSceneLoaded()
    {
        Transform parent = GameObject.FindGameObjectWithTag("ItemsParent").transform;
        if (parent != null)
            _itemsParent = parent;
        else
            _itemsParent = null;

        _currentLocation.SetDecoTilemaps();
        _grid = _currentLocation.Grid;
    }
    #region Grid

    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return _grid.WorldToCell(worldPos);
    }
    public Vector3Int WorldToGrid(Grid grid, Vector3 worldPos)
    {
        return grid.WorldToCell(worldPos);
    }
    public Vector3 GridToWorldCenter(Vector3Int gridPos)
    {
        return _grid.GetCellCenterWorld(gridPos);
    }
    public Vector3Int ScreenToGridPos(Vector2 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        return _grid.WorldToCell(worldPos);

    }

    public Vector3 ScreenToWorldPos(Vector2 mousePos)
    {
        return _grid.CellToWorld(ScreenToGridPos(mousePos));
    }

    #endregion
}
