using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapRoute 
{
    public string FromMapName;
    public string ToMapName;
    public List<MapPath> MapPaths;
}
