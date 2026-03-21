using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ManagerInfo
{
    public MonoBehaviour Manager;
    public string ManagerName;
}
public class GameManager : SingletonMonobehaviour<GameManager>
{
    public static event Action OnAllManagersReady;

    [SerializeField] ManagerInfo[] _managersToWait;
    int _managersReadyCount = 0;
    bool _allManagersReady = false;

    PlayerController _player;
    public PlayerController Player { get { return _player; } }
    public bool AllManagersReady { get { return _allManagersReady; } }
    protected override void Awake()
    {
        base.Awake();

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
