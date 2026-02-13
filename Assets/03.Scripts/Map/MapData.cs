using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;
public class MapData 
{
    GameObject _root;

    public string _mapName;
    public int _mapWidth;
    public int _mapHeight;

    public int _minX;
    public int _minY;
    public int _actualWidth;
    public int _actualHeight;

    Dictionary<string, TileData> _tiles;
    public Dictionary<string, TileData> Tiles { get { return _tiles; } }

    public MapData(GameObject rootGameObject,string mapName,int mapWidth, int mapHeight)
    {
        _root = rootGameObject;
        _mapName = mapName;
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;

        _tiles = new Dictionary<string, TileData>();
        SetTiles(rootGameObject);
        SetObjects();
        CalculateBounds();
    }

    #region Tilemap Layer
    void SetTiles(GameObject root)
    {
        Dictionary<string, Tilemap> tileMaps = GetTileMaps();
        if (tileMaps.Count == 0) return;

        SetTileData(tileMaps);
    }
    Dictionary<string, Tilemap> GetTileMaps()
    {
        Dictionary<string, Tilemap> tileMaps = new Dictionary<string, Tilemap>();

        for (int i = 0; i < MAP_LAYER_NAMES.Length; i++)
        {
            string layerName = MAP_LAYER_NAMES[i];
            if (!TryGetTileMap(layerName, out Tilemap tilemap)) continue;

            tileMaps.Add(layerName, tilemap);
        }

        return tileMaps;
    }

    bool TryGetTileMap(string layerName, out Tilemap tileMap)
    {
        Transform layerTransform = _root.transform.GetChild(0).Find(layerName);
        if (layerTransform == null)
        {
            Debug.Log($"Map Layer not found : {layerName}");
            tileMap = null;
            return false;
        }

        tileMap = layerTransform.GetComponent<Tilemap>();
        return tileMap != null;
    }
    void SetTileData(Dictionary<string, Tilemap> tileMaps)
    {
        HashSet<Vector2Int> validPositions = CollectValidPositions(tileMaps);
        MergeTileData(validPositions, tileMaps);
    }

    void MergeTileData(HashSet<Vector2Int> positions, Dictionary<string, Tilemap> tileMaps)
    {
        foreach (Vector2Int pos in positions)
        {
            TileData tileData = CreateTileData(pos, tileMaps);
            string key = GridUtils.GetTileKey(pos.x, pos.y);
            _tiles.Add(key, tileData);
        }
    }

    TileData CreateTileData(Vector2Int pos, Dictionary<string, Tilemap> tileMaps)
    {
        TileData tileData = new TileData(pos.x, pos.y);
        foreach (var kvp in tileMaps)
        {
            TileBase unityTile = kvp.Value.GetTile(new Vector3Int(pos.x, pos.y, 0));
            if (unityTile != null)
            {
                tileData.ApplyLayerDefaults(kvp.Key);
                SuperTile superTile = unityTile as SuperTile;
                tileData.ApplyTiledProperties(superTile);
            }
        }

        return tileData;
    }
    HashSet<Vector2Int> CollectValidPositions(Dictionary<string, Tilemap> tileMaps)
    {
        HashSet<Vector2Int> validPositions = new HashSet<Vector2Int>();

        foreach (var kvp in tileMaps)
        {
            string layerName = kvp.Key;
            Tilemap tilemap = kvp.Value;

            tilemap.CompressBounds();

            Vector3Int startCell = tilemap.cellBounds.min;
            Vector3Int endCell = tilemap.cellBounds.max;

            for (int x = startCell.x; x <= endCell.x; x++)
            {
                for (int y = startCell.y; y <= endCell.y; y++)
                {
                    TileBase unityTile = tilemap.GetTile(new Vector3Int(x, y, 0));

                    if (unityTile != null)
                    {
                        validPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return validPositions;
    }
    void CalculateBounds()
    {
        _minX = int.MaxValue;
        _minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var kvp in _tiles)
        {
            Vector2Int pos = GridUtils.TileKeyToVector2Int(kvp.Key);
            _minX = Mathf.Min(_minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            _minY = Mathf.Min(_minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }
        _actualWidth = maxX - _minX + 1;
        _actualHeight = maxY - _minY + 1;
    }

    #endregion

    #region Object Layer
    void SetObjects()
    {
        Dictionary<string, Transform> objectLayers = GetObjectLayers();
        SetObjectData(objectLayers);
    }

    Dictionary<string, Transform> GetObjectLayers()
    {
        Dictionary<string, Transform> objectLayers = new Dictionary<string, Transform>();

        for (int i = 0; i < MAP_LAYER_NAMES.Length; i++)
        {
            string objectLayerName = $"{MAP_LAYER_NAMES[i]} Object";
            Transform foundLayer = _root.transform.GetChild(0).Find(objectLayerName);

            if (foundLayer != null)
                objectLayers.Add(objectLayerName, foundLayer);
            else
                Debug.Log($"Object Layer not found : {foundLayer}");
        }

        return objectLayers;
    }

    void SetObjectData(Dictionary<string, Transform> objectLayers)
    {
        foreach (var kvp in objectLayers)
        {
            Transform parent = kvp.Value;
            SuperCustomProperties[] properties = parent.GetComponentsInChildren<SuperCustomProperties>();

            foreach (SuperCustomProperties propCompo in properties)
            {
                Vector3Int gridPos = GridUtils.WorldToGrid(propCompo.transform.position);
                ApplyObjectProperties(gridPos, propCompo.m_Properties);
            }
        }
    }

    void ApplyObjectProperties(Vector3Int gridPos, List<CustomProperty> properties)
    {
        TileData tileData = GetTileData(gridPos.x, gridPos.y);
        bool isNewTile = tileData == null;

        if (isNewTile)
            tileData = new TileData(gridPos.x, gridPos.y);

        foreach (CustomProperty prop in properties)
        {
            tileData.SetProperty(prop.m_Name, prop.m_Value);
        }

        if (isNewTile)
        {
            string key = GridUtils.GetTileKey(gridPos.x, gridPos.y);
            _tiles.Add(key, tileData);
        }

    }
    #endregion

    /* 맵 경계 내 위치인지 확인 */
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight;
    }
   
    public TileData GetTileData(int gridX, int gridY)
    {
        string key = GridUtils.GetTileKey(gridX,gridY);

        if (!_tiles.TryGetValue(key, out TileData tileData))
            return null;
        else 
            return tileData;
    }
}
