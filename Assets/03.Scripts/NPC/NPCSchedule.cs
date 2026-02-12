using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NPCSchedule : MonoBehaviour
{
    NPCController _npcController;

    Dictionary<string, List<ScheduleData>> _scheduleDict = new Dictionary<string, List<ScheduleData>>();
    List<ScheduleData> _currentSchedule;
    int _currentScheduleIndex = 0;
    int _lastMinute = -1;

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }

    void Start()
    {
        _npcController = GetComponent<NPCController>();
        _scheduleDict = TableDataManager.Instance.GetNPCScheduleDict(_npcController.NPCName);
    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        TimeManager.Instance.OnMinutePassed -= OnMinuteTick;
        TimeManager.Instance.OnMinutePassed += OnMinuteTick;
    }
    void OnDisable()
    {
        TimeManager.Instance.OnMinutePassed -= OnMinuteTick;
    }
    void SubscribeEvent()
    {
        TimeManager.Instance.OnMinutePassed += OnMinuteTick;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void OnMinuteTick(int gameMinute, int gameHour, int gameDay, string gameDayOfWeek, Season gameSeason, int gameYear)
    {
        int currentTime = (gameHour * 100) + gameMinute;
        if (_lastMinute == currentTime) return;
        _lastMinute = currentTime;

        /* 새 날 */
        if (gameHour == 0 && gameMinute == 0)
        {
            _currentSchedule = GetSchedule(gameSeason.ToString().ToLower(), gameDay, gameDayOfWeek);
            _currentScheduleIndex = 0;
            _npcController.ResetDay();
            return;
        }

        if (_currentSchedule == null || _currentScheduleIndex >= _currentSchedule.Count) return;

        ScheduleData nextSchedule = _currentSchedule[_currentScheduleIndex];
        if (currentTime >= nextSchedule.Time)
        {
            _npcController.MoveTo(nextSchedule);
            _currentScheduleIndex++;

            /* 취침 */
            if (nextSchedule.Time >= 2200)
                _currentScheduleIndex = _currentSchedule.Count;
        }
        
    }

    List<ScheduleData> GetSchedule(string gameSeason, int gameDay, string gameDayOfWeek)
    {
        List<string> keys = new List<string>();

        /* season_gameDay */
        if (gameDay > 0) keys.Add($"{gameSeason}_{gameDay}");

        // 계절, 요일, 날짜

        /* season_DayOfWeek */
        keys.Add($"{gameSeason}_{gameDayOfWeek}");
        keys.Add($"{gameDayOfWeek}");
        keys.Add($"{gameSeason}");
        keys.Add($"{gameDay}");

        foreach(string key in keys)
        {
            if(_scheduleDict.TryGetValue(key,out List<ScheduleData> scheduleData) && scheduleData.Count>0)
                return scheduleData;
        }

        return null;

    }
}
