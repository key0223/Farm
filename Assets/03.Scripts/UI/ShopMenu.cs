using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : ClickableMenu
{
    PlayerController _player;
    [SerializeField] GameObject _slotParent;

    [Header("Shop Info UI References")]
    [SerializeField] Image _shopIconImage;
    [SerializeField] TextMeshProUGUI _shopNameText;
    [Header("Item Info UI References")]
    [SerializeField] GameObject _itemInfoObj;
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] TextMeshProUGUI _itemCategoryText;
    [Header("Buy UI References")]
    ShopPurchase _shopPurchase;
    [SerializeField] GameObject _buyObj;
    [SerializeField] TextMeshProUGUI _buyText;
    

    string _shopSlotPrefabPath = "UI/ShopSlot";
    string _currentShopId;

    ObjectItem _selectedItem;
    List<ObjectItem> _items = new List<ObjectItem>();
    List<ShopSlot> _slots =new List<ShopSlot>();

    public Item SelectedItem { get { return _selectedItem; } }
    protected override void Awake()
    {
        base.Awake();
        _menuName = "Shop";
    }

    protected override void Start()
    {
        base.Start();
        _buyText.text = LocalizationManager.Instance.GetString("Buy");
        _shopPurchase = GetComponentInChildren<ShopPurchase>();
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

    public void SetCurrentShop(string shopId,PlayerController player)
    {
        if(_currentShopId == shopId) return;
        _currentShopId = shopId;
        _player = player;
        RefreshMenu();
    }
    public void SetSelectedItem(ObjectItem item)
    {
        if(_selectedItem == item|| item==null)
        {
            _selectedItem = null;
            _itemInfoObj.gameObject.SetActive(false);
            _buyObj.gameObject.SetActive(false);
        }
        else
        {
            _selectedItem = item;
            _itemNameText.text = LocalizationManager.Instance.GetString(_selectedItem.DisplayName);
            _itemCategoryText.text = LocalizationManager.Instance.GetString(_selectedItem.Category);
            string color = _selectedItem.CategoryColor;
            _itemCategoryText.color = Parser.ParseColor(color);

            _shopPurchase.SetItemPrice(_selectedItem.Price,_player);
            _itemInfoObj.gameObject.SetActive(true);
            _buyObj.gameObject.SetActive(true);

        }
    }
    void RefreshMenu()
    {
        if(_currentShopId == null) return;

        SetSelectedItem(null);



        ShopDataBase data;
        TableDataManager.Instance.ShopDict.TryGetValue(_currentShopId, out data);

        _shopNameText.text = LocalizationManager.Instance.GetString(data.DisplayName);

        AddForSale(data);
    }

    void AddForSale(ShopDataBase data)
    {
        foreach(string category in data.SalableCategories)
        {
            List<ObjectItem> categoryItems = TableDataManager.Instance.ItemDict.Values
                                       .Where(item => item.Category == category)
                                       .Select(data=>ItemFactory.Create(data.Id)as ObjectItem).ToList();

            _items.AddRange(categoryItems);
        }

        UpdateSlots();
    }

    void UpdateSlots()
    {
        ClearSlots();

        for (int i = 0; i < _items.Count; i++)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_shopSlotPrefabPath,_slotParent.transform);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            slot.SlotIndex = i;
            slot.ClickableId = i;
            slot.SetItem(_items[i]);

            _slots.Add(slot);

            if(!_clickableComponents.Contains(slot))
                _clickableComponents.Add(slot);
        }
       
    }
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
                if(slot != null && slot.CurrentItem!=null)
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
        foreach(ClickableComponent component in _clickableComponents)
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
        //throw new System.NotImplementedException();
    }

   
    void ClearSlots()
    {
        for (int i = _slots.Count-1; i >=0 ; i--)
        {
            if(_clickableComponents.Contains(_slots[i]))
                _clickableComponents.Remove(_slots[i]);

            _slots[i].Clear();
            ResourceManager.Instance.Destroy(_slots[i].gameObject);
        }
        
        _slots.Clear();
    }
}
