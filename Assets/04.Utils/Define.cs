using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Define 
{
    public static readonly string[] MAP_LAYER_NAMES = { "Back","Deco", "Buildings", "Path", "Front", "Always Front" };
    public const int CELL_SIZE = 1;
    public const int TILE_SIZE = 16;
    public const int ITEM_MAX_STACK = 99;

    public const string PERSISTENT_SCENE = "PersistentScene";
    public const string DEFAULT_TEXT_COLOR = "#313131";

    // Time 실제 시간 50배
    public const float SECONDS_PER_GAME_SECOND = 1 / 50f;

    public enum SceneName
    {
        Scene1_FarmHouse,
        Scene2_Farm,
        Scene3_Town,

    }
    public enum ObjectType
    {
        BASIC,
        LITTER,
        ARTIFACT,
        MINERALS,
        QUEST,
        CRAFTING,
        FISH,
        COOKING,
        SEEDS,
        INTERACTIVE,
    }
    public enum ItemType
    {
        OBJECTS,
        TOOLS,
        WEAPONS,
    }

    public enum ToolType
    {
        HANDS,
        AXE,
        ROD,
        PICKAXE,
        SHOVEL,
        WATERING,
    }
    public enum Season
    {
        SPRING,
        SUMMER,
        FALL,
        WINTER,
    }

    public enum SpriteLayer
    {
        BODY,
        HAIR,
        ARMS,
    }

    public enum TileFeatureType
    {
        NONE,
        HOE_DIRT,

    }
}
