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

    const string MasterKey = "MasterVolume";
    const string MusicKey = "MusicVolume";
    const string AmbientKey = "AmbientVolume";
    protected override void Awake()
    {
        base.Awake();
        _menuName = "Settings";
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SubsSliderEvents();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SaveSettings();
        UnsubsSliderEvents();
    }
    protected override void SubscribeEvent()
    {
        LoadVolume();
        SubsSliderEvents();
        base.SubscribeEvent();
    }
    
    void SubsSliderEvents()
    {
        UnsubsSliderEvents();
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
