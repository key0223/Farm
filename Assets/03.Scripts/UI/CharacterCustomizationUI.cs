using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationUI : MonoBehaviour, IQuantityAdjuster
{
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
        _playerNameInput.onValueChanged.AddListener(delegate { UpdateInputText(); });
        _farmNameInput.onValueChanged.AddListener(delegate { UpdateInputText(); });
        _colorSlider.onValueChanged.AddListener(OnColorSliderChanged);
        _hairColorResetButton.onClick.AddListener(OnHairColorResetButtonClicked);
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        OnHairColorResetButtonClicked();
        _confirmButton.interactable = false;
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
        _hairStyleText.text = $"¸Ó¸® {_currentHairIndex}";
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

        GameManager.Instance.Player.SetPlayerProfile(profile);
        UIManager.Instance.HideTitle();

    }
}
