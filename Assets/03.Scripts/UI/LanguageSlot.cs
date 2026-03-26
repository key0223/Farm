using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSlot : MonoBehaviour
{
    LanguageSettingUI _languageSettingUI;
    TextMeshProUGUI _languageNameText;
    Button _languageButton;

    [SerializeField] string _languageCode;

   

    public string LanguageCode { get { return _languageCode; } set { _languageCode = value; } }

    void Awake()
    {
        _languageNameText = GetComponentInChildren<TextMeshProUGUI>();
        _languageButton = GetComponentInChildren<Button>();
        _languageButton.onClick.AddListener(OnLanguageButtonClicked);
    }

    public void SetSlot(LanguageSettingUI languageSettingUI, string language)
    {
        _languageSettingUI = languageSettingUI;
        _languageCode = language;
        _languageNameText.text = GetLanguageText(language);
    }
    void OnLanguageButtonClicked()
    {
        GameManager.Instance.SetLanguage(_languageCode);
        _languageSettingUI.ShowLanguageSettingUI(false);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    string GetLanguageText(string languageCode)
    {
        switch(languageCode)
        {
            case "en":return "ENGLISH";
            case "ko":return "ÇÑ±¹¾î";
            default: return "ENGLISH";

        }
    }
}
