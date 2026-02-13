using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    [SerializeField] SO_MapRouteList _soMapRouteList;

    NPCController[] _npcArray;

    AStar _aStar;
    Dictionary<string, MapRoute> _mapRouteDict = new Dictionary<string, MapRoute>();

    protected override void Awake()
    {
        base.Awake();
        _npcArray = FindObjectsOfType<NPCController>();
        InitMapRouteDict();
        _aStar = GetComponent<AStar>();
        GameManager.Instance.ManagerReady("NPCManager");
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoad;
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoad;
    }
    void OnDisable()
    {

        GameSceneManager.Instance.OnAfterSceneLoad -= AfterSceneLoad;

    }
    void SubscribeEvent()
    {
        GameSceneManager.Instance.OnAfterSceneLoad += AfterSceneLoad;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void InitMapRouteDict()
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

    void AfterSceneLoad()
    {
        SetNPCActiveStatus();
    }

    void SetNPCActiveStatus()
    {
        foreach(NPCController npc in _npcArray)
        {
            if (npc.NPCMovement.CurrentLocation == SceneManager.GetActiveScene().name)
                npc.SetNPCActiveInScene();
            else
                npc.SetNPCInactiveInScene();
        }
    }

}
