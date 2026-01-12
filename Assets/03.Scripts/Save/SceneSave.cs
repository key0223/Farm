using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneSave /* 씬에서 필요한 모든 세부 데이터 */
{
    public Dictionary<string, int> IntDictionary;
    public Dictionary<string, bool> BoolDictionary;
    public Dictionary<string,string> StringDictionary;
    public Dictionary<string, Vector3Serializable> Vector3Dictionary;
    public List<SceneItem> SceneItemList;
    public Dictionary<string, TileFeatureSave> TileFeatureDictionary;
}
