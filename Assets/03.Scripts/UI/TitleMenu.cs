using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : ClickableMenu
{
    [Header("Refresh UI Targets")]
    [SerializeField] Image _titleLogoImage;
    [SerializeField] List<Sprite> _logoSprites;
    [Space(10)]
    [SerializeField] TextMeshProUGUI _newGameText;
    [SerializeField] TextMeshProUGUI _loadText;
    [SerializeField] TextMeshProUGUI _exitText;
    [Header("Language Button References")]
    [SerializeField] LanguageSettingUI _languageSettingUI;
    [SerializeField] Button _languageButton;
    [Header("Exit Button")]
    [SerializeField] Button _exitButton;

    [SerializeField] float _slideDuration = 1.5f;
    RectTransform _rectTransform;
    int _currentIndex;
    float[] _positions = { 1920, 0, -1920 };

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        _menuName = "Title";
        _rectTransform = GetComponent<RectTransform>();

        _languageButton.onClick.AddListener(OnLanguageButtonClicked);  
        _exitButton.onClick.AddListener(OnExitButtonClicked);
    }
    protected override void Start()
    {
        base.Start();
        RefreshUI();
        UIManager.Instance.ShowTitle();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllManagersReady)
            return;

        GameManager.OnLanguageChanged -= RefreshUI;
        GameManager.OnLanguageChanged += RefreshUI;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.OnLanguageChanged -= RefreshUI;

    }
    protected override void SubscribeEvent()
    {
        GameManager.OnLanguageChanged += RefreshUI;
        base.SubscribeEvent();
    }
   

    void RefreshUI()
    {
        string code = GameManager.Instance.Config.LanguageCode;
        _titleLogoImage.sprite = GetLogoByLanguageCode(code);

        string newGame = LocalizationManager.Instance.GetString("NewGame");
        string load = LocalizationManager.Instance.GetString("Load");
        string exit = LocalizationManager.Instance.GetString("Exit");

        _newGameText.text = newGame;
        _loadText.text = load;
        _exitText.text = exit;
    }
    public void SlideToIndex(int index)
    {
        if (_rectTransform == null || index < 0 || index >= _positions.Length)
            return;

        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosX(_positions[index], _slideDuration,snapping:true)
                   .SetEase(Ease.Linear);
        _currentIndex = index;
    }

    void OnLanguageButtonClicked()
    {
        _languageSettingUI.ShowLanguageSettingUI(true);
    }

    Sprite GetLogoByLanguageCode(string languageCode)
    {
        switch (languageCode)
        {
            case "en": return _logoSprites[0];
            case "ko": return _logoSprites[1];
            default: return _logoSprites[0];
        }
    }
    void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #region Clickable

    public override bool ShouldExitOnEscapeKey()
    {
        return false;
    }
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }

    #endregion

}
