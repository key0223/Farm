using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class RecipeSlot : ClickableComponent
{
    [Header("UI References")]
    [SerializeField] Image _lockedImage;
    [SerializeField] Image _unlockedImage;
    [SerializeField] Image _craftedImage;

    Item _currentItem;
    int _slotIndex;

    bool _unlocked = false;
    bool _crafted = false;

    bool _isHovered = false;


    public Item CurrentItem { get { return _currentItem; } }
    public int SlotIndex { get { return _slotIndex; }set { _slotIndex = value; } }
    public bool Unlocked { get { return _unlocked; } }



    public override void Start()
    {
        base.Start();
        RefreshIconImage();
    }

    public void SetItem(Item item)
    {
        _currentItem = item;
        SetIconImage();
        RefreshIconImage();

    }
    void SetIconImage()
    {
        if(_currentItem != null)
        {
            _lockedImage.sprite = _currentItem.Icon;
            _unlockedImage.sprite = _currentItem.Icon;
            _craftedImage.sprite = _currentItem.Icon;
        }
    }
    void RefreshIconImage()
    {
        if(!_unlocked)
        {
            _unlockedImage.enabled = false;
            _craftedImage.enabled = false;
        }
        else if(_unlocked && !_crafted)
        {
            _unlockedImage.enabled = true;
            _craftedImage.enabled = false;
        }
        else if(_crafted)
        {
            _craftedImage.enabled = true;
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
