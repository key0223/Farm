using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    #region Saveable

    string _isavableUniqueId;
    GameObjectSave _gameObjectSave;

    public string ISaveableUniqueId { get { return _isavableUniqueId; } set { _isavableUniqueId = value; } }
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #endregion

    public event Action<int,int,int,string,Season,int> OnMinutePassed;
    public event Action OnHourPassed;
    public event Action OnDayPassed;
    public event Action OnSeasonPassed;
    public event Action OnYearPassed;

    Season _gameSeason = Season.SPRING;
    int _gameYear = 1;
    int _gameDay = 1;
    int _gameHour = 6;
    int _gameMinute = 0;
    int _gameSecond = 0;
    string _gameDayOfWeek = "Mon";

    bool _gameClockPaused = false;
    float _gameTick = 0f;


    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        _isavableUniqueId = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();

        GameManager.Instance.ManagerReady("TimeManager");
    }

    void Start()
    {
        OnMinutePassed?.Invoke(_gameMinute, _gameHour, _gameDay, _gameDayOfWeek, _gameSeason, _gameYear);
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        ISaveableRegister();
    }
    void OnDisable()
    {
        ISaveableDeregister();
    }
    void SubscribeEvent()
    {
        ISaveableRegister();

        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void Update()
    {
        if (!_gameClockPaused)
            GameTick();

        PlayerTestInput();
    }

    void GameTick()
    {
        _gameTick += Time.deltaTime;
        if(_gameTick >= SECONDS_PER_GAME_SECOND)
        {
            _gameTick -= SECONDS_PER_GAME_SECOND;
            UpdateGameSecond();
        }
    }
    void UpdateGameSecond()
    {
        _gameSecond++;

        if (_gameSecond > 59)
        {
            _gameSecond = 0;
            _gameMinute++;

            if (_gameMinute > 59)
            {
                _gameMinute = 0;
                _gameHour++;

                if (_gameHour > 23)
                {
                    _gameHour = 0;
                    _gameDay++;

                    if (_gameDay > 30)
                    {
                        _gameDay = 1;

                        int gs = (int)_gameSeason;
                        gs++;

                        _gameSeason = (Season)gs;

                        if (gs > 4)
                        {
                            gs = 1;
                            _gameSeason = (Season)gs;

                            _gameYear++;

                            if (_gameYear > 9999)
                                _gameYear = 1;

                            OnYearPassed?.Invoke();
                        }
                        OnSeasonPassed?.Invoke();
                    }
                    _gameDayOfWeek = GetDayOfWeek();
                    OnDayPassed?.Invoke();
                }
                OnHourPassed?.Invoke();
            }
            OnMinutePassed?.Invoke(_gameMinute,_gameHour,_gameDay,_gameDayOfWeek,_gameSeason,_gameYear);
        }
    }

    string GetDayOfWeek()
    {
        int totalDays = (((int)_gameSeason) * 30) + _gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    void PlayerTestInput()
    {
        // Trigger Advance Time
        if (Input.GetKey(KeyCode.T))
        {
            TestAdvanceGameMinute();
        }

        // Trigger Advance Day
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestAdvanceGameDay();
        }

    }

    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    #region Saveable
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.ISaveableList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.ISaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(PERSISTENT_SCENE);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(ISaveableUniqueId, out GameObjectSave gameObjSave))
        {
            GameObjectSave = gameObjSave;
            ISaveableRestoreScene(PERSISTENT_SCENE);
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.SceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();
        sceneSave.IntDictionary = new Dictionary<string, int>();
        sceneSave.StringDictionary = new Dictionary<string, string>();

        sceneSave.IntDictionary.Add("gameYear", _gameYear);
        sceneSave.IntDictionary.Add("gameDay", _gameDay);
        sceneSave.IntDictionary.Add("gameHour", _gameHour);
        sceneSave.IntDictionary.Add("gameMinute", _gameMinute);
        sceneSave.IntDictionary.Add("gameSecond", _gameSecond);

        sceneSave.StringDictionary.Add("gameDayOfWeek",_gameDayOfWeek);
        sceneSave.StringDictionary.Add("gameSeason",_gameSeason.ToString());

        GameObjectSave.SceneData.Add(sceneName, sceneSave);

    }

    public void ISaveableRestoreScene(string sceneName)
    {
       if(GameObjectSave.SceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.IntDictionary != null && sceneSave.StringDictionary !=null)
            {
                if (sceneSave.IntDictionary.TryGetValue("gameYear", out int savedGameYear))
                    _gameYear = savedGameYear;

                if (sceneSave.IntDictionary.TryGetValue("gameDay", out int savedGameDay))
                    _gameDay = savedGameDay;

                if (sceneSave.IntDictionary.TryGetValue("gameHour", out int savedGameHour))
                    _gameHour = savedGameHour;

                if (sceneSave.IntDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                    _gameMinute = savedGameMinute;

                if (sceneSave.IntDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                    _gameSecond = savedGameSecond;

                // populate string saved values
                if (sceneSave.StringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                    _gameDayOfWeek = savedGameDayOfWeek;

                if (sceneSave.StringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                {
                    if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                    {
                        _gameSeason = season;
                    }
                }

                _gameTick = 0;
                OnMinutePassed?.Invoke(_gameMinute, _gameHour, _gameDay, _gameDayOfWeek, _gameSeason, _gameYear);
            }
        }
    }

    #endregion
}
