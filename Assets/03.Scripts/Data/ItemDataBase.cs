using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
[Serializable]

public class ItemDataBase
{
    public int Id;
    public string Name;
    public string DisplayName;
    public string Description;

    public ItemType ItemType;
    public string Category;
    public string CategoryColor;
    public string SheetDirectory;
    public string ParentSheet;
    public int SpriteIndex;
   
    public string Buffs;
    public string ContextTags;
    public bool Stackable;
    public bool CanBeGivenAsGift;
    public bool CanBeTrashed;

    public Sprite Icon;
}

[Serializable]
public class ObjectDataBase : ItemDataBase
{
    public ObjectType ObjectType;
    public int Price;
    public int Edibility;
    public bool IsDrink;
}

[Serializable]
public class ToolDataBase:ItemDataBase
{
    public ToolType ToolType;
    public int SalePrice;
    public int UpgradeLevel;
    public string UpgradeFromStr;

    public UpgradeFrom UpgradeFrom;

}




