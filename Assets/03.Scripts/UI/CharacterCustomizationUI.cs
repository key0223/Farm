using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CharacterCustomizationUI : MonoBehaviour, IQuantityAdjuster
{
    [Header("Refresh UI Targets")]
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _namePlaceholderText;
    [SerializeField] TextMeshProUGUI _farmNameText;
    [SerializeField] TextMeshProUGUI _farmNamePlaceholerText;
    [SerializeField] TextMeshProUGUI _resetText;
    [SerializeField] TextMeshProUGUI _hairColorText;
    [SerializeField] TextMeshProUGUI _backText;

    [Header("Name References")]
    [SerializeField] TMP_InputField _playerNameInput;
    [SerializeField] TMP_InputField _farmNameInput;

    [Header("Hair Style References")]
    [SerializeField] List<Sprite> _hairSprites = new List<Sprite>();
    [SerializeField] TextMeshProUGUI _hairStyleText;
    [SerializeField] Image _hairPreviewImage;

    string[] _hairStyleNames = { "Culy", "Spikey", "Short", "Mob", "Long", "Bowl" };

    [Header("Color References")]
    [SerializeField] Slider _colorSlider;
    [SerializeField] Button _hairColorResetButton;

    [Header("Confirm Button")]
    [SerializeField] Button _confirmButton;


    string _playerName;
    string _farmName;

    int _currentHairIndex;
    Color _selectedColor;

    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;

        _playerNameInput.onValueChanged.AddListener(delegate { UpdateInputText(); });
        _farmNameInput.onValueChanged.AddListener(delegate { UpdateInputText(); });
        _colorSlider.onValueChanged.AddListener(OnColorSliderChanged);
        _hairColorResetButton.onClick.AddListener(OnHairColorResetButtonClicked);
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        OnHairColorResetButtonClicked();
        _confirmButton.interactable = false;
    }
    void Start()
    {
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

    #region Hair
    void OnColorSliderChanged(float value)
    {
        float h = value;
        _selectedColor = Color.HSVToRGB(h, 1f, 1f);
        _hairPreviewImage.color = _selectedColor;
    }
    void OnHairColorResetButtonClicked()
    {
        _selectedColor = Color.white;
        _hairPreviewImage.color = _selectedColor;
    }    

    void RefreshHairUI()
    {
        string hair = LocalizationManager.Instance.GetString("Hair");
        _hairStyleText.text = $"{hair} {_currentHairIndex}";
        _hairPreviewImage.sprite = _hairSprites[_currentHairIndex];
    }
    public void IncreaseQuantity()
    {
        if (_currentHairIndex < _hairSprites.Count - 1)
            _currentHairIndex++;
        else
            _currentHairIndex = 0;

        RefreshHairUI();
    }

    public void DecreaseQuantity()
    {
        if (_currentHairIndex > 0)
            _currentHairIndex--;
        else
            _currentHairIndex = _hairSprites.Count - 1;

        RefreshHairUI();
    }
    #endregion

    void RefreshUI()
    {
        string name = LocalizationManager.Instance.GetString("Name");
        string inputName = LocalizationManager.Instance.GetString("InputName");
        string farmName = LocalizationManager.Instance.GetString("FarmName");
        string reset = LocalizationManager.Instance.GetString("Reset");
        string hairColor = LocalizationManager.Instance.GetString("HairColor");
        string back = LocalizationManager.Instance.GetString("Back");

        _nameText.text = name;
        _namePlaceholderText.text = inputName;
        _farmNameText.text = farmName;
        _farmNamePlaceholerText.text = farmName;
        _resetText.text = reset;
        _hairColorText.text = hairColor;
        _backText.text = back;

        RefreshHairUI();
    }
    void UpdateInputText()
    {
        _playerName = _playerNameInput != null ? _playerNameInput.text : "";
        _farmName = _farmNameInput != null ? _farmNameInput.text : "";
        CheckConfirmButton();
    }

   
    void CheckConfirmButton()
    {
        _confirmButton.interactable = _playerName.Length > 0 && _farmName.Length > 0;
    }
    void OnConfirmButtonClicked()
    {
        PlayerProfile profile = new PlayerProfile()
        {
            FarmName = _farmName,
            PlayerName = _playerName,
            HairName = _hairStyleNames[_currentHairIndex],
            HairColor = _selectedColor
        };
        SoundManager.Instance.PlaySound(SoundName.UI_CLICK_5);
        StartCoroutine(CoEnterGame(profile));
    }

    IEnumerator CoEnterGame(PlayerProfile profile)
    {
        GameSceneManager.Instance.Fade();
        yield return new WaitForSeconds(1f);
        GameManager.Instance.Player.SetPlayerProfile(profile);
        GiveDefaultItem();
        UIManager.Instance.HideTitle();
    }

    void GiveDefaultItem()
    {
        PlayerController player = GameManager.Instance.Player;
        Item shovel = ItemFactory.Create(7040);
         player.PlayerInven.TryAdd(shovel);

        Item wateringCan = ItemFactory.Create(7050);
        player.PlayerInven.TryAdd(wateringCan);

        Item seed = ItemFactory.Create(601,10);
        player.PlayerInven.TryAdd(seed);

        player.PlayerProfile.Money = 500;
    }
}
