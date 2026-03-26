using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : SingletonMonobehaviour<LocalizationManager>
{
    string _currentLanguageCode;
    Dictionary<string,StringDataBase> _currentLanguageDict = new Dictionary<string,StringDataBase>();

    public string CurrentLanguageCode {get{ return _currentLanguageCode;}set{_currentLanguageCode = value;}}

    
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;
        GameManager.Instance.ManagerReady("LocalizationManager");
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;
        GameManager.OnLanguageChanged -= SetLanguageDict;
        GameManager.OnLanguageChanged += SetLanguageDict;

    }
    void OnDisable()
    {
        GameManager.OnLanguageChanged -= SetLanguageDict;
    }
    void SubscribeEvent()
    {
        _currentLanguageCode =  GameManager.Instance.Config.LanguageCode;
        SetLanguageDict();
        GameManager.OnLanguageChanged += SetLanguageDict;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    public void SetLanguageDict()
    {
        _currentLanguageDict.Clear();
        _currentLanguageDict = GetLanguageDict(_currentLanguageCode);
    }
    public string GetString(string key)
    {
        StringDataBase data = GetStringData(key);
        if (data == null) return null;

        return data.Translation;
    }
    public string GetString(string key, params object[] arguments)
    {
        StringDataBase data = GetStringData(key);

        string text = data != null ? data.Translation : $"{key}";
        return arguments.Length>0 ? string.Format(text, arguments) : text ;
    }

    StringDataBase GetStringData(string key)
    {
        if (_currentLanguageDict == null) return null;

        StringDataBase data;
        _currentLanguageDict.TryGetValue(key, out data);
        if(data == null) return null;

        return data;
    }

    Dictionary<string,StringDataBase> GetLanguageDict(string languageCode)
    {
        Dictionary<string, StringDataBase> dict;
        TableDataManager.Instance.Languages.TryGetValue(languageCode, out dict);
        if(dict == null) return null;
        return dict;
    }

}
