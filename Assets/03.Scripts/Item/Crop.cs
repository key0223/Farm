using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Crop 
{
    int _id;
    bool _needsWatering;
    int _harvestItemId;
    int _harvestMinStack;
    int _harvestMaxStack;
    float _extraHarvestChance;

    HashSet<Season> _seasons;
    List<int> _daysInPhase;
    HashSet<ToolType> _harvestMethod;
    Sprite[] _cropSprites;

    /* 런타임 정보 */


    int _currentPhase;
    int _daysOfCurrentPhase;
    bool _fullyGrown;
    bool _dead;

    public int Id {  get { return _id; }set { _id = value; } }
    public int CurrentPhase { get {  return _currentPhase; }set { _currentPhase = value; } }
    public int DaysOfCurrentPhase { get { return _daysOfCurrentPhase; } set { _daysOfCurrentPhase = value; } }
    public bool FullyGrown { get { return _fullyGrown; }set { _fullyGrown = value; } }

    public Crop(int seedId)
    {
        CropDataBase cropData = TableDataManager.Instance.CropDict[seedId];

        _id = cropData.Id;
        _needsWatering = cropData.NeedsWatering;
        _harvestItemId = cropData.HarvestItemId;
        _harvestMinStack = cropData.HarvestMinStack;
        _harvestMaxStack = cropData.HarvestMaxStack;
        _extraHarvestChance = cropData.ExtraHarvestChance;

        _seasons = Parser.ParseEnum<Season>(cropData.SeasonsStr);
        _daysInPhase = Parser.ParseInt(cropData.DaysInPhaseStr);
        _harvestMethod = Parser.ParseEnum<ToolType>(cropData.HarvestMethodStr);
        _cropSprites = GetSprites(cropData);

        _currentPhase = 0;
        _daysOfCurrentPhase = 0;
        _fullyGrown = false;
        _dead = false;
    }
  
    public void CropUpdate()
    {
        if (_fullyGrown) return;

        /* 현재 단계 일수 >= 해당 단계 필요 일수 */
        if(_daysOfCurrentPhase >= GetPhaseDays(_currentPhase))
        {
            _daysOfCurrentPhase = 0;
            _currentPhase++;

            if(_currentPhase>=_daysInPhase.Count)
            {
                _fullyGrown=true;
                return;
            }
        }
        _daysOfCurrentPhase++;

    }

    public void OnHarvest(Vector3Int position)
    {
        int harvestCount = Random.Range(_harvestMinStack, _harvestMaxStack+1);
        float rand = Random.Range(0, 1);
        if (rand < _extraHarvestChance)
            harvestCount++;

        GameLocation location = MapManager.Instance.CurrentLocation;

        for (int i = 0; i < harvestCount; i++)
        {
            Item resultItem = ItemFactory.Create(_harvestItemId, 1);
            location.AddWorldObject(resultItem, position);
        }
    }

    public bool IsReadyForGrowth(bool wasWateredYesterday)
    {
        return !_needsWatering || wasWateredYesterday;
    }
    int GetPhaseDays(int phase)
    {
        /* 해당 단계 필요일수 */
        return _daysInPhase[phase];
    }
    public Sprite GetCurrentPhaseSprite()
    {
        return _cropSprites[_currentPhase];
    }
    Sprite[] GetSprites(CropDataBase data)
    {
        string directory = data.SheetDirectory;
        string sheet = data.ParentSheet;
        int startIndex = data.SpriteIndex;

       return LoadUtils.GetAtlasSprites(directory, sheet, startIndex,count:5);

    }
}
