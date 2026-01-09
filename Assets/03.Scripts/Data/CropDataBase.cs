using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
[Serializable]
public class CropDataBase 
{
    public int Id;
    public string SeasonsStr;
    public string DaysInPhaseStr;
    public bool NeedsWatering;
    public int HarvestItemId;
    public int HarvestMinStack;
    public int HarvestMaxStack;
    public float ExtraHarvestChance;
    public string HarvestMethodStr;
    public string SheetDirectory;
    public string ParentSheet;
    public int SpriteIndex;

    //public HashSet<Season> Seasons;
    //public List<int> DaysInPhase;
    //public HashSet<ToolType> HarvestMethod;
}
