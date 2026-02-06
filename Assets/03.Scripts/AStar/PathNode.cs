using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
/// <summary>
/// A* 경로를 Stack으로 저장
/// </summary>
public class PathNode 
{
    public string MapName;
    public int Hour;
    public int Minute;
    public int Second;
    public Vector2Int TargetGrid;

    [HideInInspector]
    public bool IsCompleted;


   
}
