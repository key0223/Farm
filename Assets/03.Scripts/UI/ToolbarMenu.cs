using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ToolbarMenu : ClickableMenu
{
    PlayerController _playerController;
    Container _playerContainer;
    List<ContainerSlot> _slots = new List<ContainerSlot>();

    int _selectedIndex = -1;

    protected override void Awake()
    {
        base.Awake();
        _playerController = GetComponent<PlayerController>();
        _menuName = "Toolbar";
    }

    protected override void Start()
    {
        base.Start();
        InitSlots();
        gameObject.SetActive(true);

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
        _playerController = FindObjectOfType<PlayerController>();
        _playerContainer = FindObjectOfType<PlayerController>().PlayerInven.PlayerContainer;

        _playerContainer.OnSlotChanged += UpdateSlots;
        base.SubscribeEvent();
    }
    void InitSlots()
    {
        ContainerSlot[] foundSlots = GetComponentsInChildren<ContainerSlot>();
        for (int i = 0; i < foundSlots.Length && i < 10; i++)
        {
            ContainerSlot slot = foundSlots[i];
            slot.OwnerContainer = _playerContainer;
            slot.SlotIndex = i;
            _slots.Add(slot);

            ClickableComponent clickable = slot.GetComponent<ClickableComponent>();
            if (clickable == null)
                clickable = slot.gameObject.AddComponent<ClickableComponent>();
            clickable.ClickableId = i;
            _clickableComponents.Add(clickable);
        }
        UpdateSlots();
        UpdateHighlightVisual();
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
        SelectSlotAtPosition(screenPos);
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }

    public override void ReceiveScrollWheel(float delta)
    {
        int direction = delta > 0 ? -1 : 1;
        _selectedIndex = Mathf.Clamp((_selectedIndex + direction + _slots.Count) % _slots.Count, 0, _slots.Count - 1);

        Item item = _playerContainer.Storage.GetItemAtSlot(_selectedIndex);
        _playerController.PlayerInven.SetCurrentToolbarItem(item);
        UpdateHighlightVisual();
    }
    public void SelectToolbarSlot(int index)
    {
        if (index <0 || index >= _slots.Count) return;

        if(_selectedIndex == index)
        {
            _selectedIndex = -1;
            _playerController.PlayerInven.SetCurrentToolbarItem(null);
        }
        else
        {
            _selectedIndex = index;
            Item item = _playerContainer.Storage.GetItemAtSlot(index);
            _playerController.PlayerInven.SetCurrentToolbarItem(item);
        }
        UpdateHighlightVisual();
    }
    void SelectSlotAtPosition(Vector2 screenPos)
    {
        foreach (ClickableComponent component in _clickableComponents)
        {
            if (component.ContainsPoint((int)screenPos.x, (int)screenPos.y))
            {
                SelectToolbarSlot(component.ClickableId);
                break;
            }
        }
    }
    public void UpdateHighlightVisual()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            Transform highlight = _slots[i].transform.Find("Selection Highlight");
            if (highlight != null)
                highlight.gameObject.SetActive(i == _selectedIndex);
        }
    }
}
