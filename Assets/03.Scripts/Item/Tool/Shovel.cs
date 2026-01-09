using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Shovel : Tool
{
    public Shovel(ToolDataBase data)
    {
        Id = data.Id;
        Name = data.Name;
        DisplayName = data.DisplayName;
        Description = data.Description;
        Category = data.Category;
        CategoryColor = data.CategoryColor;
        Stackable = data.Stackable;
        CanBeGivenAsGift = data.CanBeGivenAsGift;
        CanBeTrashed = data.CanBeTrashed;
        Tags = Parser.ParseString(data.ContextTags);
        Icon = LoadUtils.GetAtlasSprite(data.SheetDirectory,data.ParentSheet,data.SpriteIndex);

        ToolType = data.ToolType;
        UpgradeLevel = data.UpgradeLevel;
        UpgradeFrom = Parser.ParseUpgradFrom(data.SalePrice,data.UpgradeFromStr);
    }

    public override Item OnClone(Item clone)
    {
        Shovel tool = clone as Shovel;

        tool.Tags = new HashSet<string>(tool.Tags);
        return tool;
    }

    public override void UseTool(Vector3Int targetCell)
    {
        Till(targetCell);
    }

    void Till(Vector3Int targetGrid)
    {
        GameLocation location = MapManager.Instance.CurrentLocation;
        TileData targetTile = location.MapData.GetTileData(targetGrid.x, targetGrid.y);

        if (targetTile == null || !targetTile.IsDiggable)
            return;

        location.SetTileFeature(targetGrid,ToolType);

        SoundManager.Instance.PlaySound(SoundName.EFFECT_SHOVEL);
    }
}
