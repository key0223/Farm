using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : ClickableMenu
{
    [Header("Audio")]
    [SerializeField] AudioMixer _audioMixer;
    [Space(10)]
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _ambientSlider;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI _soundText;
    [SerializeField] TextMeshProUGUI _masterVolumeText;
    [SerializeField] TextMeshProUGUI _musicVolumeText;
    [SerializeField] TextMeshProUGUI _ambientVolumeText;

    [Header("Controls")]
    [SerializeField] TextMeshProUGUI _moveText;
    [SerializeField] TextMeshProUGUI _inventoryText;
    [SerializeField] TextMeshProUGUI _escText;
    [SerializeField] TextMeshProUGUI _leftClickText;
    [SerializeField] TextMeshProUGUI _rightClickText;
    [SerializeField] TextMeshProUGUI _middleClickText;
    [SerializeField] TextMeshProUGUI _hotkeyText;

    const string MasterKey = "MasterVolume";
    const string MusicKey = "MusicVolume";
    const string AmbientKey = "AmbientVolume";
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;
        _menuName = "Settings";
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
        UnsubsSliderEvents();
        SubsSliderEvents();

        GameManager.OnLanguageChanged -= RefreshUI;
        GameManager.OnLanguageChanged += RefreshUI;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SaveSettings();
        UnsubsSliderEvents();
        GameManager.OnLanguageChanged -= RefreshUI;

    }
    protected override void SubscribeEvent()
    {
        LoadVolume();
        SubsSliderEvents();
        GameManager.OnLanguageChanged += RefreshUI;
        base.SubscribeEvent();
    }
    
    void SubsSliderEvents()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _ambientSlider.onValueChanged.AddListener(SetAmbientVolume);

    }
    void UnsubsSliderEvents()
    {
        _masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        _musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        _ambientSlider.onValueChanged.RemoveListener(SetAmbientVolume);
    }
    void RefreshUI()
    {
        string sound = LocalizationManager.Instance.GetString("Sound");
        string master = LocalizationManager.Instance.GetString("MasterVolume");
        string music = LocalizationManager.Instance.GetString("MusicVolume");
        string ambient = LocalizationManager.Instance.GetString("AmbientVolume");

        _soundText.text = $"{sound}:";
        _masterVolumeText.text = master;
        _musicVolumeText.text = music;
        _ambientVolumeText.text = ambient;

        string move = LocalizationManager.Instance.GetString("Move");
        string inventory = LocalizationManager.Instance.GetString("Inventory");
        string esc = LocalizationManager.Instance.GetString("ESCDesc");
        string left = LocalizationManager.Instance.GetString("LeftClickDesc");
        string right = LocalizationManager.Instance.GetString("RightClickDesc");
        string middle = LocalizationManager.Instance.GetString("MiddleClickDesc");
        string hot = LocalizationManager.Instance.GetString("Hotkeys");

        _moveText.text = move;
        _inventoryText.text = inventory;
        _escText.text = esc;
        _leftClickText.text = left;
        _rightClickText.text = right;
        _middleClickText.text = middle;
        _hotkeyText.text = hot; 

    }
    void LoadVolume()
    {
        float master = PlayerPrefs.GetFloat(MasterKey,1f);
        float music = PlayerPrefs.GetFloat(MusicKey,1f);
        float ambient = PlayerPrefs.GetFloat(AmbientKey,1f);

        _masterSlider.SetValueWithoutNotify(master);
        _musicSlider.SetValueWithoutNotify(music);
        _ambientSlider.SetValueWithoutNotify(ambient);

        ApplyVolume("MasterVolume", master);
        ApplyVolume("MusicVolume", music);
        ApplyVolume("AmbientVolume", ambient);
    }

    void SetMasterVolume(float volume)
    {
        ApplyVolume("MasterVolume", volume);
        PlayerPrefs.SetFloat(MasterKey, volume);

    }
    void SetMusicVolume(float volume)
    {
        ApplyVolume("MusicVolume", volume);
        PlayerPrefs.SetFloat(MusicKey, volume);

    }
    void SetAmbientVolume(float volume)
    {
        ApplyVolume("AmbientVolume", volume);
        PlayerPrefs.SetFloat(AmbientKey, volume);

    }
    void ApplyVolume(string volumeName, float volume)
    {
        float db = ConvertToDecibel(volume);
        _audioMixer.SetFloat(volumeName, db);
    }
    float ConvertToDecibel(float volume)
    {
        if (volume <= 0.0001f)
            return -80f;

        return Mathf.Log10(volume) * 20f;
    }

    void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    protected override void PerformHoverAction(Vector2 mousePos)
    {
        //base.PerformHoverAction(mousePos);
    }

    public override void ReceiveLeftClick(Vector2 screenPos)
    {
    }
    public override void ReceiveRightClick(Vector2 screenPos)
    {
    }
}
