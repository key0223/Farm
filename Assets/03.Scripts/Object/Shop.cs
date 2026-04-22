using UnityEngine;
using static Define;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] string _shopId;

    int _openTime;
    int _closeTime;
    string _dayOff;
    string _closedMessage;

    bool _isDayOff = false;
    bool _isOpen = false;

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }
    void Start()
    {
        InitShop();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;

        TimeManager.Instance.OnHourPassed -= OnHourTick;
        TimeManager.Instance.OnHourPassed += OnHourTick;

        TimeManager.Instance.OnDayPassed -= OnNewDayStarted;
        TimeManager.Instance.OnDayPassed += OnNewDayStarted;


    }
    void OnDisable()
    {
        TimeManager.Instance.OnHourPassed -= OnHourTick;
        TimeManager.Instance.OnDayPassed -= OnNewDayStarted;
    }
    void SubscribeEvent()
    {
        TimeManager.Instance.OnHourPassed += OnHourTick;
        TimeManager.Instance.OnDayPassed += OnNewDayStarted;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void InitShop()
    {
        ShopDataBase data;
        TableDataManager.Instance.ShopDict.TryGetValue(_shopId, out data);
        if (data == null) return;

        _openTime = data.OpenTime;
        _closeTime = data.CloseTime;
        _dayOff = data.DayOff;
        _closedMessage = data.ClosedMessage;
    }
    public void Interact(PlayerController player)
    {
        if (_isDayOff)
        {
            string text = LocalizationManager.Instance.GetString(_closedMessage, _openTime, _closeTime);
            UIManager.Instance.ShowSimpleMessage(text);
            return;
        }
        else if (!_isOpen)
        {
            string text = LocalizationManager.Instance.GetString("Closed_OpenRange", _openTime, _closeTime);
            UIManager.Instance.ShowSimpleMessage(text);
            return;

        }
        UIManager.Instance.ShowShop(_shopId, player);
    }

    bool IsOpen(int currentTime)
    {

        if (_openTime <= _closeTime)
            return (_openTime <= currentTime) && (currentTime <= _closeTime);
        else
            return (_openTime <= currentTime) || (currentTime <= _closeTime);
    }
    void OnHourTick(int gameHour)
    {
        if (gameHour == _closeTime)
            _isOpen = false;
        else
            _isOpen = IsOpen(gameHour);
        
    }
    void OnNewDayStarted(int gameMinute, int gameHour, int gameDay, string gameDayOfWeek, Season gameSeason)
    {
        if (_dayOff == gameDayOfWeek)
        {
            _isDayOff = true;
            return;
        }
        else
            _isDayOff = false;

    }
}
