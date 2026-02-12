using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapPath 
{
    public string MapName;
    public int FromX, FromY;
    [Space(10)]
    public int ToX;
    public int ToY;

}
