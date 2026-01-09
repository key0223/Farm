using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public static class ItemFactory
{
    public static bool Exists(int id)
    {
        return TableDataManager.Instance.ItemDict.ContainsKey(id);
    }

    public static Item Create(int id, int statck = 1)
    {
        if (!Exists(id))
        {
            Debug.LogWarning($"ItemFactory : 존재하지 않는 아이템 ID {id}");
            return null;
        }

        ItemDataBase data = TableDataManager.Instance.ItemDict[id];

        switch (data.ItemType)
        {
            case ItemType.OBJECTS:
                return new ObjectItem(data, statck);
            case ItemType.TOOLS:
                return CreateTool(data);
            case ItemType.WEAPONS:
                break;

            default:
                return null;
        }

        return null;
    }

    static Tool CreateTool(ItemDataBase data)
    {
        ToolDataBase toolData = data as ToolDataBase;

        if (toolData == null)
            Debug.LogWarning($"Casting Failed. Id :{data.Id}, name : {data.Name} ");

        switch(toolData.ToolType)
        {
            case ToolType.AXE:
                break;
            case ToolType.ROD:
                break;
            case ToolType.PICKAXE:
                break;
            case ToolType.SHOVEL:
                return new Shovel(toolData);
            case ToolType.WATERING:
                return new WateringCan(toolData);
            default:
                return null;
        }

        return null;
    }
}
