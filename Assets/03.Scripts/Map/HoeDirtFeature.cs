using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HoeDirtFeature : TileRuntimeFeature
{
    Crop _currentCrop;
    CropDisplayObject _cropDisplay;
    int _daysSinceTilled;
    bool _watered;

    string _cropDisplayPath = "CropDisplay";

    public Vector3Int TilePos { get { return _tilePos; } set { _tilePos = value; } }
    public Crop CurrentCrop { get { return _currentCrop; } }
    public CropDisplayObject CropDisplay { get { return _cropDisplay; } }
    public int DaysSinceTilled { get { return _daysSinceTilled; } set { _daysSinceTilled = value; } }
    public bool Watered { get { return _watered; } set { _watered = value; } }

    public HoeDirtFeature(Vector3Int cellPos)
    {
        _tilePos = cellPos;
    }
    public override void DayUpdate(GameLocation location)
    {
        bool wateredYesterday = _watered;
        _watered = false;
        location.SetWaterGround(TilePos, false);

        /* 작물 업데이트 */
        if (_currentCrop != null)
        {
            _daysSinceTilled = 0;

            if (_currentCrop.IsReadyForGrowth(wateredYesterday))
            {
                _currentCrop.CropUpdate();
                _cropDisplay.UpdateDisplay(_currentCrop.GetCurrentPhaseSprite());
            }
        }
        else
        {
            _daysSinceTilled++;
        }

       
    }
    public override void OnRemove(GameLocation location)
    {
        base.OnRemove(location);

    }

    public override void ApplyTool(GameLocation location,ToolType toolType)
    {
       if(toolType == ToolType.PICKAXE)
        {
            string key = GridUtils.GetTileKey(TilePos.x, TilePos.y);
            /* 기존 정보 제거 */
            location.RemoveRuntimeFeature(key);
        }
       else if(toolType == ToolType.WATERING)
        {
            _watered = true;
            location.SetWaterGround(TilePos,true);
        }
    }
    public override bool CanApplyTool(ToolType tool)
    {
        return (tool == ToolType.PICKAXE ||
                tool == ToolType.WATERING);
    }

    public void Plant(GameLocation location,int seedId)
    {
        Crop crop = new Crop(seedId);
        _currentCrop = crop;

        GameObject displayObj = ResourceManager.Instance.Instantiate(_cropDisplayPath);
        Vector3Int pos = TilePos;
        displayObj.transform.position = GridUtils.GridToWorldCenter(pos);
        _cropDisplay = displayObj.GetComponent<CropDisplayObject>();
        _cropDisplay.UpdateDisplay(crop.GetCurrentPhaseSprite());
    }
    public void Harvest()
    {
        _currentCrop.OnHarvest(TilePos);
        ResourceManager.Instance.Destroy(_cropDisplay.gameObject);
        _cropDisplay = null;
        _currentCrop = null;
    }
}
