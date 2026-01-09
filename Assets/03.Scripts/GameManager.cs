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
    bool _allMamagersReady = false;

    public bool AllMamagersReady { get { return _allMamagersReady; } }
    protected override void Awake()
    {
        base.Awake();

        ValidateManagers();
    }

    public void ManagerReady(string managerName)
    {
        _managersReadyCount++;

        if (_managersReadyCount >= _managersToWait.Length)
        {
            OnAllManagersReady?.Invoke();
            _allMamagersReady = true;
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
