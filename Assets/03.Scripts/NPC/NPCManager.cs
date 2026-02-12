using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    [SerializeField] SO_MapRouteList _soMapRouteList;

    AStar _aStar;
    Dictionary<string, MapRoute> _mapRouteDict = new Dictionary<string, MapRoute>();

    protected override void Awake()
    {
        base.Awake();
        Init();
        _aStar = GetComponent<AStar>();
        GameManager.Instance.ManagerReady("NPCManager");
    }

    void Init()
    {
        if(_soMapRouteList.MapRouteList.Count > 0)
        {
            foreach(MapRoute route in  _soMapRouteList.MapRouteList)
            {
                string fromMapName = route.FromMapName;
                string toMapName = route.ToMapName;
                if (_mapRouteDict.ContainsKey(fromMapName + toMapName))
                    continue;

                _mapRouteDict.Add(fromMapName + toMapName, route);
            }
        }
    }
    public bool BuildPath(GameLocation gameLocation,Vector2Int start, Vector2Int goal, Stack<PathNode> stack)
    {
        if (_aStar.BuildPath(gameLocation, start, goal, stack))
            return true;
        else return false;
    }

    public MapRoute GetMapRoute(string fromMapName, string toMapName)
    {
        MapRoute mapRoute;
        if(_mapRouteDict.TryGetValue(fromMapName + toMapName, out mapRoute))
            return mapRoute;
        else return null;
    }
  
}
