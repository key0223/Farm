using SuperTiled2Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData 
{
    public int _gridX;
    public int _gridY;
    public int _tileIndex;
    public Dictionary<string, string> _properties;

    HashSet<string> _protectedProperties;

    public TileData(int gridX,int gridY)
    {
        _gridX = gridX;
        _gridY = gridY;
        _tileIndex = -1;

        _properties = new Dictionary<string, string>();
        _protectedProperties = new HashSet<string>();
    }
    public void ApplyLayerDefaults(string layerName)
    {
        switch (layerName)
        {
            case "Back":
                {
                    SetPropertyIfNotSet("Passable", "T");
                }
                break;
            case "Buildings":
                {
                    SetProperty("Passable", "F");
                    _protectedProperties.Add("Passable"); /* 더 이상 값을 바꾸지않게 잠금 */
                }
                break;
            case "Path":
                break;
            case "Front":
                break;
            case "Always Front":
                break;
            default:
                break;
        }
    }

    public void ApplyTiledProperties(SuperTile tile)
    {
        if(tile == null) return;

        _tileIndex = tile.m_TileId;
       
        foreach(CustomProperty prop in tile.m_CustomProperties)
        {
            /* 잠금된 속성 건너뛰기 */
            if (_protectedProperties.Contains(prop.m_Name))
                continue;

            _properties[prop.m_Name] = prop.m_Value;
        }
    }
    public bool IsPassable { get { return GetProperty("Passable") == "T"; } }
    public bool IsDiggable { get { return GetProperty("Diggable") == "T"; } }
    public bool IsWater { get { return GetProperty("Water") == "T"; } }

    bool HasProperty(string propertyName)
    {
        return _properties.ContainsKey(propertyName);
    }
    public string GetProperty(string propertyName)
    {
        return _properties.GetValueOrDefault(propertyName);
    }
    public void SetProperty(string propertyName, string value)
    {
        if (string.IsNullOrEmpty(value))
            _properties.Remove(propertyName);
        else
            _properties[propertyName] = value;
    }

    void SetPropertyIfNotSet(string propertyName, string value)
    {
        if (!HasProperty(propertyName))
            SetProperty(propertyName, value);
    }
}
