using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScheduleDataBase 
{
    public string ScheduleId;
    public string ScheduleString;

    public List<ScheduleData> scheduleDatas;
}

[Serializable]
public class ScheduleData
{
    public int Time;
    public string Location;
    public int TargetX;
    public int TargetY;
    public int Facing;
    public string Animation;
    public string DialogueId;
}
