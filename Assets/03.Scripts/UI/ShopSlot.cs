using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : ClickableComponent
{
    [Header("UI References")]
    [SerializeField] Image _iconImage;

    Item _currentItem;
    int _slotIndex = 0;

    public Item CurrentItem { get { return _currentItem; } }
    public int SlotIndex { get { return _slotIndex; } set { _slotIndex = value; } }

    public override void Start()
    {
        base.Start();
        UpdateVisual();
    }
    public void SetItem(Item item)
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
}
