using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class SaveFileSelectMenu : MonoBehaviour
{
    public event Action OnRefreshUI;

    [Header("Refresh UI Targets")]
    [SerializeField] TextMeshProUGUI _backText;

    [SerializeField] Transform _slotParent;
    string _saveFileSlotPrefabPath = "UI/SaveFileSlot";

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }
    void Start()
    {
        SetSaveFileSlots();
        RefreshUI();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;

        GameManager.OnLanguageChanged -= RefreshUI;
        GameManager.OnLanguageChanged += RefreshUI;
    }
    void OnDisable()
    {
        GameManager.OnLanguageChanged -= RefreshUI;

    }
    void SubscribeEvent()
    {
        GameManager.OnLanguageChanged += RefreshUI;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void SetSaveFileSlots()
    {
        List<string> saveFiles = SaveLoadManager.Instance.GetAllSaveFiles();

        foreach (string saveFile in saveFiles)
        {
            GameSave gameSave = SaveLoadManager.Instance.LoadGameSave(saveFile);
            if (gameSave == null)
                continue;

            GameObject slotObj = ResourceManager.Instance.Instantiate(_saveFileSlotPrefabPath);
            slotObj.transform.SetParent(_slotParent);
            SaveFileSlot saveFileSlot = slotObj.GetComponent<SaveFileSlot>();
            saveFileSlot.SetSlot(this,gameSave);
        }
    }

    void RefreshUI()
    {
        string back = LocalizationManager.Instance.GetString("Back");
        _backText.text = back;
        OnRefreshUI?.Invoke();
    }
}
