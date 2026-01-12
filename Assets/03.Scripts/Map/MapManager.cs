using SuperTiled2Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static Define;

public class MapManager : SingletonMonobehaviour<MapManager>, ISaveable
{
    #region Saveable

    string _isavableUniqueId;
    GameObjectSave _gameObjectSave;

    bool _isFirstTimeSceneLoaded = true;
    public string ISaveableUniqueId { get { return _isavableUniqueId; } set { _isavableUniqueId = value; } }
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #endregion

    public event Action<GameLocation> OnLocationChanged;

    [SerializeField] SuperMap[] _superMaps;
    [Header("Tiles")]
    [SerializeField] TileBase _dugTile;
    [SerializeField] TileBase _wateredTile;
    Dictionary<string, GameLocation> _gameLocations = new Dictionary<string, GameLocation>();

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
        RegisterLocations();

        GameManager.OnAllManagersReady += SubscribeEvent;

        _isavableUniqueId = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();

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

        ISaveableRegister();

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoaded;

        TimeManager.Instance.OnDayPassed -= AdvanceDay;
        TimeManager.Instance.OnDayPassed += AdvanceDay;
    }
    void OnDisable()
    {
        ISaveableDeregister();

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoaded;
        TimeManager.Instance.OnDayPassed -= AdvanceDay;
    }

    void SubscribeEvent()
    {
        ISaveableRegister();

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
        }
    }

    void Init()
    {
        string startSceneName = GetMapName(GameSceneManager.Instance.StartScene.ToString());

        foreach (var kvp in _gameLocations)
        {
            string mapName = kvp.Key;

            if (startSceneName == mapName)
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
        _gameLocations.TryGetValue(name, out GameLocation location);
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

    #region Saveable
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.ISaveableList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.ISaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        /* 현재 씬 데이터 저장 */
        ISaveableStoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(ISaveableUniqueId, out GameObjectSave gameObjSave))
        {
            GameObjectSave = gameObjSave;
            /* 현재 씬 데이터 복구 */
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.SceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();
        string mapName = GetMapName(sceneName);

        GameLocation location = GetLocation(mapName);
        if (location != null)
        {
            sceneSave.TileFeatureDictionary = new Dictionary<string, TileFeatureSave>();
            foreach (var kvp in location.RuntimeFeature)
            {
                if (kvp.Value is HoeDirtFeature hoeDirt)
                {
                    string key = GridUtils.GetTileKey(hoeDirt.TilePos.x, hoeDirt.TilePos.y);
                    TileFeatureSave saveData = new TileFeatureSave
                    {
                        FeatureType = TileFeatureType.HOE_DIRT,
                        GridX = hoeDirt.TilePos.x,
                        GridY = hoeDirt.TilePos.y,
                        Data = new int[6]
                    };
                    saveData.Data[0] = hoeDirt.DaysSinceTilled;
                    saveData.Data[1] = hoeDirt.Watered ? 1 : 0;
                    saveData.Data[2] = hoeDirt.CurrentCrop?.Id ?? -1;
                    saveData.Data[3] = hoeDirt.CurrentCrop?.CurrentPhase ?? 0;
                    saveData.Data[4] = hoeDirt.CurrentCrop?.DaysOfCurrentPhase ?? 0;
                    saveData.Data[5] = hoeDirt.CurrentCrop?.FullyGrown ?? false?1 : 0;

                    sceneSave.TileFeatureDictionary.Add(key, saveData);
                }
            }


            sceneSave.SceneItemList = new List<SceneItem>();
            foreach (WorldObjectItem item in location.WorldObjects.Values)
            {
                SceneItem sceneItem = new SceneItem();
                sceneItem.ItemId = item.Item.Id;
                sceneItem.Position = new Vector3Serializable(item.CellPos.x, item.CellPos.y, item.CellPos.z);
                sceneSave.SceneItemList.Add(sceneItem);
            }

            GameObjectSave.SceneData.Add(sceneName, sceneSave);

        }
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        string mapName = GetMapName(sceneName);
        GameLocation location = GetLocation(mapName);

        if (location != null)
            location.ResetToDefaultState();
        if (GameObjectSave.SceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.TileFeatureDictionary != null)
            {
                foreach (TileFeatureSave saveData in sceneSave.TileFeatureDictionary.Values)
                {
                    Vector3Int tilePos = new Vector3Int(saveData.GridX, saveData.GridY, 0);
                    string key = GridUtils.GetTileKey(saveData.GridX, saveData.GridY);

                    switch (saveData.FeatureType)
                    {
                        case TileFeatureType.HOE_DIRT:
                            {
                                HoeDirtFeature feature = new HoeDirtFeature(tilePos);
                                feature.DaysSinceTilled = saveData.Data[0];
                                feature.Watered = saveData.Data[1] == 1;

                                if (saveData.Data[2] > 0)
                                {
                                    feature.Plant(location, saveData.Data[2]);
                                    if (feature.CurrentCrop != null)
                                    {
                                        feature.CurrentCrop.CurrentPhase = saveData.Data[3];
                                        feature.CurrentCrop.DaysOfCurrentPhase = saveData.Data[4];
                                        feature.CurrentCrop.FullyGrown = saveData.Data[5] == 1;

                                    }
                                }

                                location.AddRuntimeFeature(key, feature);
                                location.SetDugGround(tilePos);

                                if (feature.Watered)
                                    location.SetWaterGround(tilePos, true);
                            }
                            break;
                    }
                }
            }


            if (_isFirstTimeSceneLoaded && sceneSave.SceneItemList!=null)
            {
                foreach (SceneItem sceneItem in sceneSave.SceneItemList)
                {
                    Item item = ItemFactory.Create(sceneItem.ItemId);
                    Vector3Int cellPos = new Vector3Int((int)sceneItem.Position.X, (int)sceneItem.Position.Y, 0);
                    location.AddWorldObject(item, cellPos);
                }
            }

            if (sceneSave.BoolDictionary != null && sceneSave.BoolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
                _isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;

            if (_isFirstTimeSceneLoaded == true)
                _isFirstTimeSceneLoaded = false;
        }
    }

    #endregion

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

