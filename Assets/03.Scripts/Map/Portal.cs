using System;
using UnityEngine;
using static Define;

[Serializable]
public class Portal 
{
    public SceneName FromMap;
    public int FromX, FromY;
    public SceneName ToMap;
    public int ToX, ToY;

}
