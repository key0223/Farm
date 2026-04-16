using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingInventorySlot : ClickableComponent
{
    [Header("UI References")]
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _stackText;

    Container _ownerContainer;
    Item _currentItem;

    int _slotIndex;
    bool _isHovered = false;

    public Container OwnerContainer { get { return _ownerContainer; } set { _ownerContainer = value; } }
    public Item CurrentItem { get { return _currentItem; } }
    public int SlotIndex { get { return _slotIndex; } set { _slotIndex = value; } }

    public override void Start()
    {
        base.Start();
    }

    public void SetItem(Item item)
    {
        _currentItem = item;
        RefreshUI();
    }

    void RefreshUI()
    {
        if(_currentItem != null)
        {
            _iconImage.sprite = _currentItem.Icon;
            _stackText.text = _currentItem.Stack > 1 ? _currentItem.Stack.ToString() : "";
            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled =false;
            _stackText.text = "";
        }
    }
    public override void OnHover()
    {
        if (_isHovered) return;
        _isHovered = true;
    }
    public override void OnHoverExit()
    {
        if (!_isHovered) return;
        _isHovered = false;

        UIManager.Instance.HideTooltip();
    }
}
