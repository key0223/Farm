using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSettingUI : MonoBehaviour
{
    [SerializeField] Transform _slotParent;
    string _languageSlotPrefabPath = "UI/LanguageSlot";
    string[] _languages = { "en", "ko" };

    void Awake()
    {
        GameManager.OnAllManagersReady += InitSlots;
    }


    void InitSlots()
    {
        foreach (string language in _languages)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_languageSlotPrefabPath);
            slotObj.transform.SetParent(_slotParent);

            LanguageSlot slot = slotObj.GetComponent<LanguageSlot>();
            slot.SetSlot(this,language);

        }
        gameObject.SetActive(false);
        GameManager.OnAllManagersReady -= InitSlots;

    }

    public void ShowLanguageSettingUI(bool show)
    {
        gameObject.SetActive(show);
    }
}
