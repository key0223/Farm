using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class TableDataManager : SingletonMonobehaviour<TableDataManager>
{
    const string rootPath = "Data";

    public Dictionary<int, ItemDataBase> ItemDict = new Dictionary<int, ItemDataBase>();
    public Dictionary<int, CropDataBase> CropDict = new Dictionary<int, CropDataBase>();
    public Dictionary<int, RecipeDataBase> RecipeDict = new Dictionary<int, RecipeDataBase>();
    public Dictionary<string, AnimationDataBase> AnimationDict = new Dictionary<string, AnimationDataBase>();
    public Dictionary<string, Dictionary<string, StringDataBase>> Languages = new Dictionary<string, Dictionary<string, StringDataBase>>();
    public Dictionary<string,Dictionary<string, DialogueData>> DialogueDict = new Dictionary<string, Dictionary<string, DialogueData>>();
    public Dictionary<string, List<ScheduleData>> ScheduleDict = new Dictionary<string, List<ScheduleData>>();
    public Dictionary<string,ShopDataBase> ShopDict = new Dictionary<string, ShopDataBase>();

    protected override void Awake()
    {
        base.Awake();
        Init();
        GameManager.Instance.ManagerReady("TableDataManager");

    }
    public void Init()
    {
        Dictionary<int, ItemDataBase> tools = LoadJson<Data.ItemToolLoader, int, ItemDataBase>("Tools").MakeDict();
        Dictionary<int, ItemDataBase> objects = LoadJson<Data.ItemLoader, int, ItemDataBase>("Objects").MakeDict();
        ItemDict = MergeDict<int, ItemDataBase>(tools, objects);
        CropDict = LoadJson<Data.CropLoader, int, CropDataBase>("Crops").MakeDict();
        RecipeDict = LoadJson<Data.RecipeLoader, int, RecipeDataBase>("Recipes").MakeDict();
        LoadAllAnimations();
        LoadAllLanguages();

        LoadAllDialogues();
        ScheduleDict = LoadJson<Data.ScheduleLoader, string, List<ScheduleData>>("ScheduleData_Rand").MakeDict();
        ShopDict = LoadJson<Data.ShopLoader, string, ShopDataBase>("ShopData").MakeDict();
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

    #region Animation

    public Dictionary<string, AnimationDataBase> GetNPCAnimationDict(string npcName)
    {
        string prefix = $"{npcName}_";
        Dictionary<string, AnimationDataBase> dict = new Dictionary<string, AnimationDataBase>();

        foreach (var kvp in AnimationDict)
        {
            if (!kvp.Key.StartsWith(npcName, StringComparison.OrdinalIgnoreCase))
                continue;

            string newKey = kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? kvp.Key.Substring(prefix.Length)
                : kvp.Key;

            dict[newKey] = kvp.Value;
        }
        return dict;
    }

    void LoadAllAnimations()
    {
        Dictionary<string, AnimationDataBase> player = LoadJson<Data.AnimationLoader, string, AnimationDataBase>("AnimationData_Player").MakeDict();
        Dictionary<string, AnimationDataBase> villagerWoman = LoadJson<Data.AnimationLoader, string, AnimationDataBase>("AnimationData_MiniVillagerWoman").MakeDict();

        AnimationDict = MergeDict<string, AnimationDataBase>(player, villagerWoman);
    }

    #endregion

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
        Dictionary<string, StringDataBase> uis = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_UI_Ko").MakeDict();
        Dictionary<string, StringDataBase> locations = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Location_Ko").MakeDict();


        result = MergeDict<string, StringDataBase>(objects, tools,uis,locations);

        return result;
    }
    Dictionary<string, StringDataBase> MakeEnglishStringDict()
    {
        Dictionary<string, StringDataBase> result = new Dictionary<string, StringDataBase>();

        Dictionary<string, StringDataBase> objects = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Objects_En").MakeDict();
        Dictionary<string, StringDataBase> tools = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Tools_En").MakeDict();
        Dictionary<string, StringDataBase> uis = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_UI_En").MakeDict();
        Dictionary<string, StringDataBase> locations = LoadJson<Data.StringLoader, string, StringDataBase>("StringData_Location_En").MakeDict();

        result = MergeDict<string, StringDataBase>(objects, tools, uis,locations);

        return result;
    }
    #endregion

    void LoadAllDialogues()
    {
        DialogueDict.Add("ko", MakeKoreanDialogueDict());
    }

    Dictionary<string,DialogueData> MakeKoreanDialogueDict()
    {
        Dictionary<string,DialogueData> result = new Dictionary<string,DialogueData>();

        Dictionary<string, DialogueData> miniVillagerWoman = LoadJson<Data.DialogueLoader, string, DialogueData>("DialogueData_MiniVillagerWoman_ko").MakeDict();

        result = MergeDict<string, DialogueData>(miniVillagerWoman);

        return result;
    }
    public Dictionary<string,DialogueData> GetNpcDialogueDict(string npcName)
    {
        Dictionary<string, DialogueData> currentDict = new Dictionary<string, DialogueData>();
        DialogueDict.TryGetValue(GameManager.Instance.Config.LanguageCode, out currentDict);

        if (currentDict == null) return null;

        string prefix = $"{npcName}_";
        Dictionary<string,DialogueData> dict = new Dictionary<string,DialogueData>();

        foreach(var kvp in currentDict)
        {
            if (!kvp.Key.StartsWith(npcName, StringComparison.OrdinalIgnoreCase))
                continue;

            string newKey = kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
               ? kvp.Key.Substring(prefix.Length)
               : kvp.Key;

            dict[newKey] = kvp.Value;
        }
        return dict;
    }
    public Dictionary<string, List<ScheduleData>> GetNPCScheduleDict(string npcName)
    {
        string prefix = $"{npcName}_";
        Dictionary<string, List<ScheduleData>> dict = new Dictionary<string, List<ScheduleData>>();

        foreach (var kvp in ScheduleDict)
        {
            if (!kvp.Key.StartsWith(npcName, StringComparison.OrdinalIgnoreCase))
                continue;

            string newKey = kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? kvp.Key.Substring(prefix.Length)
                : kvp.Key;

            dict[newKey] = kvp.Value;
        }
        return dict;
    }
}
