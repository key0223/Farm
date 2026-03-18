using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : ClickableComponent
{
    public event Action<ObjectItem> OnSlotClicked;

    [Header("UI References")]
    [SerializeField] Image _iconImage;


    ObjectItem _currentItem;
    int _slotIndex = 0;
    bool _isHovered = false;

    public Item CurrentItem { get { return _currentItem; } }
    public int SlotIndex { get { return _slotIndex; } set { _slotIndex = value; } }

    public override void Start()
    {
        base.Start();
        UpdateVisual();
    }
    public void SetItem(ObjectItem item)
    {
        _currentItem = item;
        UpdateVisual();

    }

    void UpdateVisual()
    {
        if(_currentItem != null )
        {
            _iconImage.sprite = _currentItem.Icon;

            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled=false;
        }
    }

    public void Clear()
    {
        _currentItem = null;
        _slotIndex = -1;
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

    public override void OnLeftClick(Vector2 pos)
    {
        UIManager.Instance.ShopUI.SetSelectedItem(_currentItem);
        OnSlotClicked?.Invoke(_currentItem);
        Debug.Log("Shop slot Clicked");
    }
}
