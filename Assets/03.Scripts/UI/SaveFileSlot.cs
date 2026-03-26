using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using static Define;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class SaveFileSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _farmNameText;
    [SerializeField] TextMeshProUGUI _farmText;
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _currentDayText;
    [SerializeField] TextMeshProUGUI _moneyText;

    Button _button;

    string _farmName;
    string _playerName;

    Season _gameSeason;
    int _gameYear = 1;
    int _gameDay = 1;
    GameSave _gameSave;

    void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetSlot(GameSave gameSave)
    {
        _gameSave = gameSave;

        foreach(GameObjectSave gameObjectSave in gameSave.GameObjectData.Values)
        {
            SceneSave sceneSave;
            gameObjectSave.SceneData.TryGetValue(PERSISTENT_SCENE, out sceneSave);
            if (sceneSave == null) return;

            string farmName;
            sceneSave.StringDictionary.TryGetValue("farmName", out farmName);
            if (farmName == null) return;

            SetNameText(sceneSave);
            SetDayData(sceneSave);
            break;
        }
    }

    void SetNameText(SceneSave sceneSave)
    {
        var stringDict = sceneSave.StringDictionary;
        if (stringDict == null) return;

        if (sceneSave.StringDictionary.TryGetValue("farmName", out string farmName))
        {
            _farmNameText.text = farmName;
        }
        if (sceneSave.StringDictionary.TryGetValue("playerName", out string playerName))
        {
            _playerNameText.text = playerName;
        }
    }
    void SetDayData(SceneSave sceneSave)
    {
        var intDict = sceneSave.IntDictionary;
        var stringDict = sceneSave.StringDictionary;

        if (intDict == null || stringDict == null) return;

        if (sceneSave.IntDictionary.TryGetValue("gameYear", out int savedGameYear))
            _gameYear = savedGameYear;

        if (sceneSave.IntDictionary.TryGetValue("gameDay", out int savedGameDay))
            _gameDay = savedGameDay;

        if (sceneSave.StringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
        {
            if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                _gameSeason = season;
        }

        SetDayText(LocalizationManager.Instance.CurrentLanguageCode);

    }

    void SetDayText(string languageCode)
    {
        switch (languageCode)
        {
            case "en":
                {
                    string text = $"Day {_gameDay} of {_gameSeason.ToString()}, Year {_gameYear}";
                    _currentDayText.text = text;
                }
                break;
            case "ko":
                {
                    string text = $"{_gameYear}년째, {_gameSeason.ToString()}의 {_gameDay}일째";
                    _currentDayText.text = text;
                }
                break;

        }
    }

    void SetCharacterLookData(SceneSave sceneSave)
    {
        var stringDict = sceneSave.StringDictionary;
        if (stringDict == null) return;


    }
    void OnButtonClicked()
    {
        SaveLoadManager.Instance.LoadData(_gameSave);
        UIManager.Instance.HideTitle();
    }
}
