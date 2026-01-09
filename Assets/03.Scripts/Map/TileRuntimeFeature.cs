using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class TileRuntimeFeature
{
    protected Vector3Int _tilePos;

    public virtual void DayUpdate(GameLocation location) { }
    public virtual void OnPlaced(GameLocation location) { }
    public virtual void OnRemove(GameLocation location) { }
    public virtual void ApplyTool(GameLocation location,ToolType toolType) { }
    public virtual bool CanApplyTool(ToolType tool) { return false; }
}
