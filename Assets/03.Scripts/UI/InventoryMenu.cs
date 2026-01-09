using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryMenu : ClickableMenu
{
    Container _playerContainer;

    List<ContainerSlot> _slots = new List<ContainerSlot>();
    protected override void Awake()
    {
        base.Awake();
        _playerContainer = FindObjectOfType<PlayerController>().PlayerInven.PlayerContainer;
        _menuName = "Inventory";
    }

    protected override void Start()
    {
        base.Start();
        InitSlots();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllMamagersReady)
            return;

        UpdateSlots();
        _playerContainer.OnSlotChanged -= UpdateSlots;
        _playerContainer.OnSlotChanged += UpdateSlots;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        _playerContainer.OnSlotChanged -= UpdateSlots;
    }
    protected override void SubscribeEvent()
    {
        _playerContainer.OnSlotChanged += UpdateSlots;
        base.SubscribeEvent();
    }
    void InitSlots()
    {
        ContainerSlot[] foundSlots = GetComponentsInChildren<ContainerSlot>();
       
        for (int i = 0; i < foundSlots.Length; i++)
        {
            ContainerSlot slot = foundSlots[i];
            slot.OwnerContainer = _playerContainer;
            slot.SlotIndex = i;
            _slots.Add(slot);

            ClickableComponent clickable = slot.GetComponent<ClickableComponent>();
            if(clickable == null)
                clickable = slot.gameObject.AddComponent<ClickableComponent>();
            clickable.ClickableId = slot.SlotIndex;
            _clickableComponents.Add(clickable);
        }
        UpdateSlots();
    }

    void UpdateSlots()
    {
        foreach(ContainerSlot slot in _slots)
        {
            Item item = _playerContainer.Storage.GetItemAtSlot(slot.SlotIndex);
            slot.SetItem(item);
        }
    }
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
       
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }
   
}
