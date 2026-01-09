using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
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
        GameManager.Instance.ManagerReady("TimeManager");
    }

    void Start()
    {
        OnMinutePassed?.Invoke(_gameMinute, _gameHour, _gameDay, _gameDayOfWeek, _gameSeason, _gameYear);
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
}
