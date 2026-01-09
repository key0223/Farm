using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class TableDataManager : SingletonMonobehaviour<TableDataManager>
{
    const string rootPath = "Data";

    public Dictionary<int,ItemDataBase> ItemDict = new Dictionary<int,ItemDataBase>();
    public Dictionary<int,CropDataBase> CropDict = new Dictionary<int,CropDataBase>();
    public Dictionary<string,AnimationDataBase> AnimationDict = new Dictionary<string,AnimationDataBase>();
    public Dictionary<string,Dictionary<string,StringDataBase>> Languages = new Dictionary<string, Dictionary<string, StringDataBase>>();

    protected override void Awake()
    {
        base.Awake();
        Init();
        GameManager.Instance.ManagerReady("TableDataManager");

    }
    public void Init()
    {
        Dictionary<int,ItemDataBase> tools = LoadJson<Data.ItemToolLoader, int, ItemDataBase>("Tools").MakeDict();
        Dictionary<int,ItemDataBase> objects = LoadJson<Data.ItemLoader, int, ItemDataBase>("Objects").MakeDict();
        ItemDict = MergeDict<int,ItemDataBase>(tools, objects);
        CropDict = LoadJson<Data.CropLoader, int, CropDataBase>("Crops").MakeDict();
        AnimationDict = LoadJson<Data.AnimationLoader, string, AnimationDataBase>("AnimationData_Player").MakeDict();
        LoadAllLanguages();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{rootPath}/{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    Dictionary<TKey, TValue> MergeDict<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
    {
        Dictionary<TKey, TValue> mergeDict = new Dictionary<TKey, TValue>();

        foreach (var dict in dictionaries)
        {
            foreach (var kvp in dict)
            {
                mergeDict[kvp.Key] = kvp.Value;
            }
        }

        return mergeDict;
    }

    #region String Table

    void LoadAllLanguages()
    {
        Languages.Add("ko", MakeKoreanStringDict());
        Languages.Add("en", MakeEnglishStringDict());
    }
    Dictionary<string, StringDataBase> MakeKoreanStringDict()
    {
        Dictionary<string, StringDataBase> result = new Dictionary<string, StringDataBase>();

        Dictionary<string, StringDataBase> objects = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Objects_Ko").MakeDict();
        Dictionary<string, StringDataBase> tools = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Tools_Ko").MakeDict();

        result = MergeDict<string, StringDataBase>(objects, tools);

        return result;
    }
    Dictionary<string, StringDataBase> MakeEnglishStringDict()
    {
        Dictionary<string, StringDataBase> result = new Dictionary<string, StringDataBase>();

        Dictionary<string, StringDataBase> objects = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Objects_En").MakeDict();
        Dictionary<string, StringDataBase> tools = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Tools_En").MakeDict();

        result = MergeDict<string, StringDataBase>(objects, tools);

        return result;
    }
    #endregion
}
