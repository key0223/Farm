using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingCollectionMenu : ClickableMenu
{
    RecipeRequirementEvaluator _evaluator;
    [Header("UI References")]
    [SerializeField] GameObject _slotParent;
    [SerializeField] string _recipeSlotPrefabPath = "UI/RecipeSlot";

    List<Item> _recipes = new List<Item>();
    List<RecipeSlot> _slots = new List<RecipeSlot>();
    protected override void Awake()
    {
        base.Awake();
        _evaluator = new RecipeRequirementEvaluator();
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

    void RefreshSlots()
    {
        for (int i = 0; i < _recipes.Count; i++)
        {
            Item item = _recipes[i];
            bool hasRecipe = GameManager.Instance.Player.PlayerInven.HasRecipe(_recipes[i].Id);
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
                    UIManager.Instance.ShowTooltip(slot.CurrentItem, mousePos,true);
                    return;
                }
                if (slot != null && slot.CurrentItem != null)
                {
                    UIManager.Instance.ShowTooltip(slot.CurrentItem, mousePos);
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
