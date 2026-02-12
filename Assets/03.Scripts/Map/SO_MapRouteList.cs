using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "so_MapRouteList", menuName = "Scriptable Objects/Map Route List")]
public class SO_MapRouteList : ScriptableObject
{
    public List<MapRoute> MapRouteList;
}
