using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : SingletonMonobehaviour<LocalizationManager>
{
    string _currentLanguageCode = "ko";
    Dictionary<string,StringDataBase> _currentLanguageDict = new Dictionary<string,StringDataBase>();
    
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.ManagerReady("LocalizationManager");
    }

    void Start()
    {
        SetLanguage(_currentLanguageCode);
    }

    public void SetLanguage(string languageCode)
    {
        _currentLanguageDict = GetLanguageDict(languageCode);
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
