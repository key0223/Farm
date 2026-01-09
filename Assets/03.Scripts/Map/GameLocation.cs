using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using static Define;

public class GameLocation  
{
    MapData _mapData;

    Tilemap _deco1;
    Tilemap _deco2;

    Grid _grid;

    Dictionary<string, TileRuntimeFeature> _runtimeFeature;
    Dictionary<string, WorldObjectItem> _worldObjects;

    public MapData MapData { get { return _mapData; } }
    public Grid Grid { get { return _grid; } }
    public GameLocation(SuperMap superMap)
    {
        _runtimeFeature = new Dictionary<string, TileRuntimeFeature>();
        _worldObjects = new Dictionary<string, WorldObjectItem>();
        _grid = superMap.transform.GetComponentInChildren<Grid>();
        RegisterMap(superMap);
    }

    public void ResetToDefaultState()
    {
        _deco1.ClearAllTiles();
        _deco2.ClearAllTiles();
        if(_runtimeFeature.Values.Count> 0)
        {
            foreach (string key in _runtimeFeature.Keys.ToList())
                RemoveRuntimeFeature(key);
            _runtimeFeature.Clear();
        }
        
        if(_worldObjects.Values.Count> 0)
        {
            foreach (WorldObjectItem item in _worldObjects.Values.ToList())
                GameObject.Destroy(item.gameObject);

            _worldObjects.Clear();
        }
    }
    void RegisterMap(SuperMap superMap)
    {
        CreateOverlayTilemap(superMap);

        string mapName = superMap.gameObject.name;
        int width = superMap.m_Width;
        int height = superMap.m_Height;

        _mapData = new MapData(superMap.gameObject, mapName, width, height);
        
    }

    public void SetDecoTilemaps()
    {
        if (_deco1 != null && _deco2 != null) return;

        _deco1 = GameObject.FindGameObjectWithTag("Deco1").GetComponent<Tilemap>();
        _deco2 = GameObject.FindGameObjectWithTag("Deco2").GetComponent<Tilemap>();
    }
    void CreateOverlayTilemap(SuperMap superMap)
    {
        Transform parent = superMap.gameObject.transform;
        Grid grid = parent.GetChild(0).GetComponent<Grid>();
        if (grid == null) return;

        //GameObject decoObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Map/DecoGrid"));

        //_deco1 = decoObj.transform.GetChild(0).GetComponent<Tilemap>();
        //_deco2 = decoObj.transform.GetChild(1).GetComponent<Tilemap>();
    }

    public TileRuntimeFeature GetRuntimeFeature(int gridX, int gridY)
    {
        string key = "x" + gridX + "y" + gridY;

        _runtimeFeature.TryGetValue(key, out TileRuntimeFeature runtimeFeature);
        return runtimeFeature;
    }

    public void SetTileFeature(Vector3Int cellPos,ToolType toolType)
    {
        string key = GridUtils.GetTileKey(cellPos.x, cellPos.y);
        TileRuntimeFeature existing = GetRuntimeFeature(cellPos.x, cellPos.y);

        /* 기존 feature가 있을 때 */
        if(existing?.CanApplyTool(toolType) == true)
        {
            existing.ApplyTool(this, toolType);
            //SetDugGround(cellPos,remove:true);
            return;
        }

        /* 새 Feature 생성 */
        if(toolType == ToolType.SHOVEL && existing == null)
        {
            HoeDirtFeature hoeDirtFeature = new HoeDirtFeature(cellPos);
            AddRuntimeFeature(key, hoeDirtFeature);
            SetDugGround(cellPos);
        }
    }

    void AddRuntimeFeature(string key, TileRuntimeFeature feature)
    {
        _runtimeFeature.Add(key,feature);
        feature.OnPlaced(this);
    }
    public void RemoveRuntimeFeature(string key)
    {
        if (_runtimeFeature.TryGetValue(key, out TileRuntimeFeature runtimeFeature))
        {
            runtimeFeature.OnRemove(this);
            _runtimeFeature.Remove(key);

            Vector3Int pos = GridUtils.GetCellPosFromKey(key);
            _deco1.SetTile(pos, null);
            _deco2.SetTile(pos, null);
        }
    }

    public void DayUpdateAll()
    {
        foreach (TileRuntimeFeature runtimeFeature in _runtimeFeature.Values)
        {
            runtimeFeature.DayUpdate(this);
        }
    }

    #region World Drop Items

    public WorldObjectItem AddWorldObject(Item item, Vector3Int gridPos)
    {
        string tileKey = GridUtils.GetTileKey(gridPos.x, gridPos.y);
        string key = $"{tileKey}_{item.Id}";

        if (_worldObjects.TryGetValue(key, out WorldObjectItem existing))
        {
            existing.Item.Stack += item.Stack;
            existing.Item.Stack = Mathf.Min(existing.Item.Stack, Define.ITEM_MAX_STACK);
            return existing;
        }

        GameObject obj = ResourceManager.Instance.Instantiate("WorldObjectItem");
        WorldObjectItem worldObjectItem = obj.GetComponent<WorldObjectItem>();

        Vector3 worldPos = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
        worldObjectItem.Init(this, item, gridPos);
        _worldObjects.Add(key, worldObjectItem);

        return worldObjectItem;
    }

    public void RemoveWorldObjects(Vector3Int gridPos, int itemId = -1)
    {
        string tileKey = GridUtils.GetTileKey(gridPos.x, gridPos.y);

        if (itemId != -1)
        {
            string key = $"{tileKey}_{itemId}";
            if (_worldObjects.TryGetValue(key, out WorldObjectItem worldItem))
            {
                ResourceManager.Instance.Destroy(worldItem.gameObject);
                _worldObjects.Remove(key);
            }
            return;
        }

        List<string> keysToRemove = _worldObjects.Keys.Where(k => k.StartsWith(tileKey + "_")).ToList();
        foreach (string key in keysToRemove)
        {
            if (_worldObjects.TryGetValue(key, out WorldObjectItem worldItem))
            {
                ResourceManager.Instance.Destroy(worldItem.gameObject);
                _worldObjects.Remove(key);
            }
        }
    }

    public List<WorldObjectItem> GetPickupableItem(Vector3Int playerGrid, int range = 3)
    {
        List<WorldObjectItem> result = new List<WorldObjectItem>();

        foreach (WorldObjectItem worldItem in _worldObjects.Values)
        {
            int manhattandDist = Mathf.Abs(worldItem.CellPos.x - playerGrid.x) +
                                 Mathf.Abs(worldItem.CellPos.y - playerGrid.y);

            if (manhattandDist <= range)
                result.Add(worldItem);
        }

        /* 월드 거리순 정렬 */
        return result.OrderBy(item => Vector3.Distance(
            GridUtils.GridToWorldCenter(playerGrid),
            GridUtils.GridToWorldCenter(item.CellPos))).ToList();
    }
    #endregion

    public void SetDugGround(Vector3Int cellPos, bool remove = false)
    {
        string key = GridUtils.GetTileKey(cellPos.x, cellPos.y);
        TileBase tile = !remove ? MapManager.Instance.DugTile : null;
        _deco1.SetTile(cellPos, tile);
    }
    public void SetWaterGround(Vector3Int cellPos, bool watered)
    {
        TileBase tile = watered ? MapManager.Instance.WateredTile : null;
        _deco2.SetTile(cellPos, tile);
    }
}
