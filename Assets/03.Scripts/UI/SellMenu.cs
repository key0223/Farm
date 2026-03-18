using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellMenu : ClickableMenu, IQuantityAdjuster
{
    [Header("Sell UI References")]
    [SerializeField] GameObject _slotParent;
    [SerializeField] string _shopSlotPrefabPath = "UI/ShopSlot";

    [Header("Sell UI References")]
    [SerializeField] GameObject _sellObj;
    [SerializeField] TextMeshProUGUI _sellText;
    [SerializeField] TextMeshProUGUI _quantityText;
    [SerializeField] TextMeshProUGUI _totalPriceText;
    [SerializeField] Button _sellButton;

    PlayerController _player;

    int _itemPrice;
    int _totalPrice;

    int _minQuantity = 1;
    int _maxQuantity = 0;
    int _currentQuantity = 1;

    ObjectItem _selectedItem;
    List<ObjectItem> _items = new List<ObjectItem>();
    List<ShopSlot> _slots = new List<ShopSlot>();

    protected override void Awake()
    {
        base.Awake();
        _menuName = "Sell";
    }

    protected override void Start()
    {
        base.Start();
        _sellButton.onClick.AddListener(OnSell);
        _sellText.text = LocalizationManager.Instance.GetString("Sell");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllManagersReady)
            return;

        RefreshMenu();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void SubscribeEvent()
    {
        _player = FindObjectOfType<PlayerController>();
        base.SubscribeEvent();
    }
    public void SetSelectedItem(ObjectItem item)
    {
        if (_selectedItem == item || item == null)
        {
            _selectedItem = null;
            _sellObj.gameObject.SetActive(false);
        }
        else
        {
            _selectedItem = item;
            _maxQuantity = item.Stack;
            SetItemPrice(_selectedItem.Price);
            _sellObj.gameObject.SetActive(true);
        }
    }
    public void RefreshMenu()
    {
        if (UIManager.Instance.ShopUI.CurrentShopId == null) return;
        SetSelectedItem(null);
        ShopDataBase data;
        TableDataManager.Instance.ShopDict.TryGetValue(UIManager.Instance.ShopUI.CurrentShopId, out data);

        AddInvenItems();
    }
    void AddInvenItems()
    {
        _items.Clear();
        Item[] slots = _player.PlayerInven.PlayerContainer.Storage.Slots;
        foreach (Item item in slots)
        {
            ObjectItem objectItem = item as ObjectItem;

            if (objectItem != null)
            {
                _items.Add(objectItem);
            }
        }

        UpdateSlots();
    }

    void UpdateSlots()
    {
        ClearSlot();
        for (int i = 0; i < _items.Count; i++)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_shopSlotPrefabPath, _slotParent.transform);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            slot.SlotIndex = i;
            slot.ClickableId = i;
            slot.SetItem(_items[i]);

            slot.OnSlotClicked += SetSelectedItem;

            _slots.Add(slot);

            if (!_clickableComponents.Contains(slot))
                _clickableComponents.Add(slot);
        }
    }

    void ClearSlot()
    {
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            if (_clickableComponents.Contains(_slots[i]))
                _clickableComponents.Remove(_slots[i]);

            _slots[i].OnSlotClicked -= SetSelectedItem;
            _slots[i].Clear();
            ResourceManager.Instance.Destroy(_slots[i].gameObject);
        }

        _slots.Clear();
    }
    #region Sell
    public void SetItemPrice(int price)
    {
        _itemPrice = price;
        Refresh();
    }
    public void DecreaseQuantity()
    {
        if (_currentQuantity < _maxQuantity)
        {
            _currentQuantity++;
            Refresh();
        }
    }

    public void IncreaseQuantity()
    {
        if (_currentQuantity > _minQuantity)
        {
            _currentQuantity--;
            _totalPrice = (_currentQuantity * _itemPrice);
            Refresh();
        }
    }
    void OnSell()
    {
        bool success = UIManager.Instance.ShopUI.Player.PlayerInven.TryRemove(_selectedItem.Id, _currentQuantity);
        if (success)
        {
            int rest = _selectedItem.Stack - _currentQuantity;
            if (rest > 0)
            {
                ObjectItem target = _items.Find(item => item == _selectedItem);
                if (target != null)
                    target.Stack = rest;
            }
            else
                _items.Remove(_selectedItem);
            UpdateSlots();
            Debug.Log("Sell Succeed");
        }
        else
        {
            Debug.Log("Sell Failed");

        }
    }

    void Refresh()
    {
        _totalPrice = (_currentQuantity * _itemPrice);
        _quantityText.text = _currentQuantity.ToString();
        _totalPriceText.text = _totalPrice.ToString();
    }
    #endregion
    protected override void PerformHoverAction(Vector2 mousePos)
    {
        ClickableComponent previousHover = _currentClickableComponent;
        _currentClickableComponent = null;

        if (previousHover != null)
            previousHover.OnHoverExit();

        foreach (ClickableComponent component in _clickableComponents)
        {
            bool contains = component.ContainsPoint((int)mousePos.x, (int)mousePos.y);

            if (contains)
            {
                _currentClickableComponent = component;
                component.OnHover();

                ShopSlot slot = component.GetComponent<ShopSlot>();
                if (slot != null && slot.CurrentItem != null)
                {
                    string name = LocalizationManager.Instance.GetString(slot.CurrentItem.DisplayName);
                    string itemType = slot.CurrentItem.Category;
                    string desc = LocalizationManager.Instance.GetString(slot.CurrentItem.Description);
                    string color = slot.CurrentItem.CategoryColor;
                    UIManager.Instance.ShowTooltip(name, itemType, color, desc, mousePos);
                }

                return;
            }
        }
    }
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        foreach (ClickableComponent component in _clickableComponents)
        {
            bool contains = component.ContainsPoint((int)screenPos.x, (int)screenPos.y);
            if (contains)
            {
                component.OnLeftClick(screenPos);
                break;
            }
        }
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //
    }

}
