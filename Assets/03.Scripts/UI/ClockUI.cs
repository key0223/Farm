using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class ClockUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _dateText;
    [SerializeField] TextMeshProUGUI _seasonText;
    [SerializeField] TextMeshProUGUI _timeText;

    string _currentDayOfWeek;
    string _localizedDayOfWeek;
    string _currentSeason;
    string _localizedSeason;


    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;

        TimeManager.Instance.OnMinutePassed -= UpdateGameTime;
        TimeManager.Instance.OnMinutePassed += UpdateGameTime;
    }

    void OnDisable()
    {
        TimeManager.Instance.OnMinutePassed -= UpdateGameTime;
    }
    void SubscribeEvent()
    {
        TimeManager.Instance.OnMinutePassed += UpdateGameTime;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void UpdateGameTime(int minute, int hour, int day, string gameDayOfWeek, Season season, int year)
    {
        minute = minute - (minute % 10);
        string ampm = "";
        string minStr;

        if (hour >= 12)
            ampm = " pm";
        else
            ampm = " am";

        if (hour >= 13)
            hour -= 12;

        if (minute < 10)
            minStr = "0" + minute.ToString();
        else
            minStr = minute.ToString();

        string time = hour.ToString() + " : " + minStr + ampm;
        if (_currentDayOfWeek == null || _currentDayOfWeek != gameDayOfWeek)
        {
            _currentDayOfWeek = gameDayOfWeek;
            _localizedDayOfWeek = LocalizationManager.Instance.GetString(gameDayOfWeek);
        }
        if(_currentSeason == null || _currentSeason != season.ToString())
        {
            _currentSeason = season.ToString();
            _localizedSeason = LocalizationManager.Instance.GetString(_currentSeason);
        }
        _dateText.text = $"{_localizedDayOfWeek}. {day}";
        _seasonText.text = _localizedSeason;
        _timeText.text = time;
    }

}
