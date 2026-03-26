using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ManagerInfo
{
    public MonoBehaviour Manager;
    public string ManagerName;
}
public class GameManager : SingletonMonobehaviour<GameManager>
{
    public static event Action OnLanguageChanged;
    public static event Action OnAllManagersReady;

    string _configPath;
    GameConfig _config;

    [SerializeField] ManagerInfo[] _managersToWait;
    int _managersReadyCount = 0;
    bool _allManagersReady = false;

    PlayerController _player;

    public GameConfig Config { get { return _config; } }
    public PlayerController Player { get { return _player; } }
    public bool AllManagersReady { get { return _allManagersReady; } }
    protected override void Awake()
    {
        base.Awake();
        _configPath = Path.Combine(Application.persistentDataPath,"config.json");
        _config = new GameConfig();
        LoadConfig();

        ValidateManagers();
        _player =  FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SaveLoadManager.Instance.SaveDataToFile();
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            SaveLoadManager.Instance.LoadDataFromFile();
            Debug.Log("Load Data");
        }
    }

    public void SaveConfig()
    {
        string json = JsonUtility.ToJson(_config, true);
        File.WriteAllText(_configPath, json);
    }

    public void LoadConfig()
    {
        if(File.Exists(_configPath))
        {
            string json = File.ReadAllText(_configPath);
            _config = JsonUtility.FromJson<GameConfig>(json);
        }
        else
            SaveConfig();
    }
    public void SetLanguage(string language)
    {
        _config.LanguageCode = language;
        SaveConfig();
        OnLanguageChanged?.Invoke();
    }
    
    public void ManagerReady(string managerName)
    {
        _managersReadyCount++;

        if (_managersReadyCount >= _managersToWait.Length)
        {
            OnAllManagersReady?.Invoke();
            _allManagersReady = true;
        }
    }
    void ValidateManagers()
    {
        foreach (ManagerInfo manager in _managersToWait)
        {
            if (manager.Manager == null)
                Debug.LogError($"GameManager: {manager.ManagerName} ´©¶ô");
        }
    }
}
