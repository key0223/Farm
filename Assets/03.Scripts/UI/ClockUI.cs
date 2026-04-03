using DG.Tweening;
using TMPro;
using UnityEngine;
using static Define;

public class ClockUI : MonoBehaviour
{
    [Header("Clock")]
    [SerializeField] TextMeshProUGUI _dateText;
    [SerializeField] TextMeshProUGUI _seasonText;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] TextMeshProUGUI _amPmText;

    string _currentDayOfWeek;
    string _localizedDayOfWeek;
    string _currentSeason;
    string _localizedSeason;

    [Header("Money")]
    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] float _moneyAnimDuration = 0.5f;

    int _currentMoney;
    Tween _moneyTween;




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

        GameManager.Instance.Player.OnMoneyChanged += RefreshMoney;
        GameManager.Instance.Player.OnMoneyChanged -= RefreshMoney;
    }

    void OnDisable()
    {
        TimeManager.Instance.OnMinutePassed -= UpdateGameTime;
        GameManager.Instance.Player.OnMoneyChanged -= RefreshMoney;

    }
    void SubscribeEvent()
    {
        TimeManager.Instance.OnMinutePassed += UpdateGameTime;
        GameManager.Instance.Player.OnMoneyChanged += RefreshMoney;

        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void UpdateGameTime(int minute, int hour, int day, string gameDayOfWeek, Season season, int year)
    {
        minute = minute - (minute % 10);

        string minStr;

        SetAmPm(hour);

        if (hour >= 13)
            hour -= 12;

        if (minute < 10)
            minStr = "0" + minute.ToString();
        else
            minStr = minute.ToString();

        string time = hour.ToString() + " : " + minStr;
        if (_currentDayOfWeek == null || _currentDayOfWeek != gameDayOfWeek)
        {
            _currentDayOfWeek = gameDayOfWeek;
            _localizedDayOfWeek = LocalizationManager.Instance.GetString(gameDayOfWeek);
        }
        if (_currentSeason == null || _currentSeason != season.ToString())
        {
            _currentSeason = season.ToString();
            _localizedSeason = LocalizationManager.Instance.GetString(_currentSeason);
        }
        _dateText.text = $"{_localizedDayOfWeek}. {day}";
        _seasonText.text = _localizedSeason;
        _timeText.text = time;
    }

    void RefreshMoney(int money)
    {
        SoundManager.Instance.PlaySound(SoundName.UI_CHANGE_DROP);
        _moneyTween?.Kill();

        float startValue = _currentMoney != 0 ? _currentMoney : 0;
        float targetValue = money;

        _moneyTween = DOTween.To(() => startValue, x =>
        {
            startValue = x;
            _moneyText.text = ((int)x).ToString();
        },
        targetValue, _moneyAnimDuration)
            .SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _currentMoney = money;
                _moneyText.text = money.ToString();
            });
        _moneyText.text = money.ToString();
    }

    void SetAmPm(int hour)
    {
        string ampm = "";

        if (hour >= 12)
            ampm = "Pm";
        else
            ampm = "Am";

        _amPmText.text = LocalizationManager.Instance.GetString(ampm);

    }

}
