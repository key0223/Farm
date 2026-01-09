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

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
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

        string time = hour.ToString() + " : " +minStr + ampm;

        _dateText.text = $"{gameDayOfWeek}. {day}";
        _seasonText.text = season.ToString();
        _timeText.text = time;
    }

}
