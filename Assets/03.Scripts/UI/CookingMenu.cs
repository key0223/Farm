using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingMenu : ClickableMenu
{
    [Header("UI References")]
    [SerializeField] GameObject _cookingSlotParent;
    [SerializeField] string _cookingSlotPrefabPath = "UI/CookingSlot";

    PlayerController _playerController;
    Container _playerContainer;

    List<Item> _cookingItems = new List<Item>();
    List<Recipe> _recipes = new List<Recipe>();
    CookingSlot[] _cookingSlots;
    CookingInventorySlot[] _cookingInventorySlots;

    protected override void Awake()
    {
        _menuName = "Cooking";
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _cookingSlots = new CookingSlot[TableDataManager.Instance.RecipeDict.Count];
        Init_CookingItems();
        Init_CookingSlots();
        Init_InventorySlot();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void SubscribeEvent()
    {
        _playerController = GameManager.Instance.Player;
        _playerContainer = GameManager.Instance.Player.PlayerInven.PlayerContainer;
        base.SubscribeEvent();
    }
    void Init_CookingItems()
    {
        _cookingItems.Clear();
        List<Item> items = TableDataManager.Instance.ItemDict.Values
                           .Where(recipe => recipe.Category == "Cooking")
                           .Select(data => ItemFactory.Create(data.Id)).ToList();

        _cookingItems.AddRange(items);

        Init_Recipes();
    }
    void Init_Recipes()
    {
        _recipes.Clear();
        foreach (Item item in _cookingItems)
        {
            // Cooking Item Id 가 아니라 레시피 ID 넘겨야함
            Recipe recipe = new Recipe(item.Id);
            _recipes.Add(recipe);
        }
    }

    void Init_CookingSlots()
    {
        ClearCookingSlots();
        for (int i = 0; i < _cookingItems.Count; i++)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_cookingSlotPrefabPath, _cookingSlotParent.transform);
            CookingSlot slot = slotObj.GetComponent<CookingSlot>();
            slot.SlotIndex = i;
            slot.SetItem(_cookingItems[i]);

            _cookingSlots[i] = slot;

            if (!_clickableComponents.Contains(slot))
                _clickableComponents.Add(slot);
        }
    }

    void Init_InventorySlot()
    {
        CookingInventorySlot[] foundSlots = GetComponentsInChildren<CookingInventorySlot>();
        _cookingInventorySlots = new CookingInventorySlot[foundSlots.Length];

        for(int i =0; i <foundSlots.Length; i++)
        {
            CookingInventorySlot slot = foundSlots[i];
            slot.OwnerContainer = _playerContainer;
            slot.SlotIndex = i;
            _cookingInventorySlots[i]= slot;

            if(!_clickableComponents.Contains(slot))
            _clickableComponents.Add(slot);
        }

        RefreshInventorySlots();
    }
    void RefreshInventorySlots()
    {
        foreach(CookingInventorySlot slot in _cookingInventorySlots)
        {
            Item item = _playerContainer.Storage.GetItemAtSlot(slot.SlotIndex);
            slot.SetItem(item);
        }
    }
    void ClearCookingSlots()
    {
        if (_cookingSlots[0] == null) return;
        for (int i = 0; i < _cookingSlots.Length; i++)
        {
            if (_clickableComponents.Contains(_cookingSlots[i]))
                _clickableComponents.Remove(_cookingSlots[i]);

            ResourceManager.Instance.Destroy(_cookingSlots[i].gameObject);
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

                CookingInventorySlot slot = component.GetComponent<CookingInventorySlot>();
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
        ClickableComponent previousHover = _currentClickableComponent;
        _currentClickableComponent = null;

        foreach (ClickableComponent component in _clickableComponents)
        {
            bool contains = component.ContainsPoint((int)screenPos.x, (int)screenPos.y);

            if (contains)
            {
                _currentClickableComponent = component;

            }
        }
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }
}
