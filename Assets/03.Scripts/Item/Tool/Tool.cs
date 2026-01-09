using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class Tool : Item
{
    ToolType _toolType;
    int _upgradeLevel;
    UpgradeFrom _updateFrom;

    public override int Stack { get { return _stack; } set { _stack = value; } }
    public ToolType ToolType { get { return _toolType; } set { _toolType = value; } }
    public int UpgradeLevel { get { return _upgradeLevel; } set {_upgradeLevel = value; } }
    public UpgradeFrom UpgradeFrom { get { return _updateFrom; } set { _updateFrom = value; } }

    public override Item OnClone(Item clone)
    {
        Tool tool = clone as Tool;

        tool.ToolType = _toolType;
        tool.UpgradeLevel = _upgradeLevel;
        tool._updateFrom = _updateFrom;

        return tool;
    }
    public abstract void UseTool(Vector3Int targetCell);
}
