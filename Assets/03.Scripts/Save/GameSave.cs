using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave 
{
    // key = ISaveableUniqueId
    public Dictionary<string, GameObjectSave> GameObjectData;
    public GameSave()
    {
        GameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
