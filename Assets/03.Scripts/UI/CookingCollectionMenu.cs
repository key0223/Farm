using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingCollectionMenu : ClickableMenu
{
    [Header("UI References")]
    [SerializeField] GameObject _slotParent;
    [SerializeField] string _recipeSlotPrefabPath = "UI/RecipeSlot";

    List<Item> _recipes = new List<Item>();
    List<RecipeSlot> _slots = new List<RecipeSlot>();
    protected override void Awake()
    {
        base.Awake();
        _menuName = "CookingCollection";
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);
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
        Init_RecipeItems();
        base.SubscribeEvent();
    }

    void Init_RecipeItems()
    {
        _recipes.Clear();
        List<Item> items = TableDataManager.Instance.ItemDict.Values
                           .Where(recipe => recipe.Category == "Cooking")
                           .Select(data => ItemFactory.Create(data.Id)).ToList();

        _recipes.AddRange(items);

        Init_Slots();
    }
    void Init_Slots()
    {
        ClearSlots();
        for (int i = 0; i < _recipes.Count; i++)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_recipeSlotPrefabPath, _slotParent.transform);
            RecipeSlot slot = slotObj.GetComponent<RecipeSlot>();
            slot.SlotIndex = i;
            slot.SetItem(_recipes[i]);

            _slots.Add(slot);

            if(!_clickableComponents.Contains(slot))
                _clickableComponents.Add(slot);
        }
    }

    void ClearSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_clickableComponents.Contains(_slots[i]))
                _clickableComponents.Remove(_slots[i]);

            ResourceManager.Instance.Destroy(_slots[i].gameObject);
        }

        _slots.Clear();
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

                RecipeSlot slot = component.GetComponent<RecipeSlot>();

                if(!slot.Unlocked)
                {
                    string name =ConvertToQuestionMark(LocalizationManager.Instance.GetString(slot.CurrentItem.DisplayName));
                    string itemType = ConvertToQuestionMark(slot.CurrentItem.Category);
                    string desc = ConvertToQuestionMark(LocalizationManager.Instance.GetString(slot.CurrentItem.Description));
                    string color = slot.CurrentItem.CategoryColor;
                    UIManager.Instance.ShowTooltip(name, itemType, color, desc, mousePos);
                    return;
                }
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

    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
    }

    string ConvertToQuestionMark(string str)
    {
        return new string('?', str.Length);
    }
}
