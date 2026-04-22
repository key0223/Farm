using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ExitMenu : ClickableMenu
{
    [Header("Goto Title")]
    [SerializeField] Button _exitToTitleButton;
    [SerializeField] TextMeshProUGUI _exitToTitleText;

    [Header("Exit Game")]
    [SerializeField] Button _exitGameButton;
    [SerializeField] TextMeshProUGUI _exitGameText;


    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        _menuName = "Exit";
    }

    protected override void Start()
    {
        base.Start();
        RefreshUI();
        gameObject.SetActive(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UnsubsButtonEvents();
        SubsButtonEvents();

        GameManager.OnLanguageChanged -= RefreshUI;
        GameManager.OnLanguageChanged += RefreshUI;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UnsubsButtonEvents();
        GameManager.OnLanguageChanged -= RefreshUI;

    }
    protected override void SubscribeEvent()
    {
        SubsButtonEvents();
        GameManager.OnLanguageChanged += RefreshUI;

        base.SubscribeEvent();
    }
    void SubsButtonEvents()
    {
        _exitToTitleButton.onClick.AddListener(OnExitToTitleButtonClicked);
        _exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
    }

    void UnsubsButtonEvents()
    {
        _exitToTitleButton.onClick.RemoveListener(OnExitToTitleButtonClicked);
        _exitGameButton.onClick.RemoveListener(OnExitGameButtonClicked);
    }

    void RefreshUI()
    {
        string title = LocalizationManager.Instance.GetString("ExitToTitle");
        string exitGame = LocalizationManager.Instance.GetString("ExitGame");

        _exitToTitleText.text = title;
        _exitGameText.text = exitGame;
    }

    void OnExitToTitleButtonClicked()
    {
        SoundManager.Instance.PlaySound(SoundName.UI_CLICK_3);
        StartCoroutine(CoFade());
    }
    void OnExitGameButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator CoFade()
    {
        GameSceneManager.Instance.Fade();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.HandleEscape();
        UIManager.Instance.ShowTitle();

    }
    protected override void PerformHoverAction(Vector2 mousePos)
    {
    }

    public override void ReceiveLeftClick(Vector2 screenPos)
    {
    }
    public override void ReceiveRightClick(Vector2 screenPos)
    {
    }
}
