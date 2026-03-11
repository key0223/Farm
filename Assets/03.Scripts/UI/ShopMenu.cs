using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : ClickableMenu
{
    List<ShopSlot> _slots;

    protected override void Awake()
    {
        base.Awake();
        _menuName = "Shop";
    }

    protected override void Start()
    {
        base.Start();
        InitSlots();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllManagersReady)
            return;

    }
    protected override void OnDisable()
    {
        base.OnDisable();

    }
    protected override void SubscribeEvent()
    {
        base.SubscribeEvent();
    }

    void InitSlots()
    {
        ShopSlot[] foundSlots = GetComponentsInChildren<ShopSlot>();

        for (int i = 0; i < foundSlots.Length; i++)
        {
            ShopSlot slot = foundSlots[i];
            slot.SlotIndex = i;
            slot.ClickableId = i;
            _slots.Add(slot);
            
            if(!_clickableComponents.Contains(slot))
                _clickableComponents.Add(slot);

        }
        UpdateSlots();
    }

    void UpdateSlots()
    {
        foreach(ShopSlot slot in _slots)
        {
        }
    }
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }

   
}
