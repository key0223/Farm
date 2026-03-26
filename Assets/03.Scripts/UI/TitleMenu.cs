using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : ClickableMenu
{
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
        _menuName = "Title";
        _rectTransform = GetComponent<RectTransform>();

        _languageButton.onClick.AddListener(OnLanguageButtonClicked);  
        _exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    protected override void Start()
    {
        base.Start();
        UIManager.Instance.ShowTitle();

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
    void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #region Clickable
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
