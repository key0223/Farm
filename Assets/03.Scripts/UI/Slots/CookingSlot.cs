using UnityEngine.UI;
using UnityEngine;


public class CookingSlot : ClickableComponent
{
    [Header("UI References")]
    [SerializeField] Image _lockedImage;
    [SerializeField] Image _unlockedImage;
    [SerializeField] Image _canMakeImage;

    Item _currentItem;
    RecipeDataBase _recipeData;
    int _slotIndex;

    bool _unlocked = false;
    bool _canMake = false;

    bool _isHovered = false;


    public Item CurrentItem { get { return _currentItem; } }
    public int SlotIndex { get { return _slotIndex; }set { _slotIndex = value; } }
    public bool Unlocked { get { return _unlocked; } }
    public bool CanMake { get { return _canMake; } }



    public override void Start()
    {
        base.Start();
        RefreshIconImage();
    }

    public void SetItem(Item item,bool canMake,bool hasRecipe)
    {
        int recipeId = TableDataManager.Instance.GetRecipeId(item.Id);
        RecipeDataBase recipeData;
        TableDataManager.Instance.RecipeDict.TryGetValue(recipeId, out recipeData);
        if (recipeData == null) return;

        _currentItem = item;
        _canMake = canMake;
        _unlocked = hasRecipe;
        _recipeData = recipeData;

        SetIconImage();
        RefreshIconImage();

    }
    void SetIconImage()
    {
        if(_currentItem != null)
        {
            _lockedImage.sprite = _currentItem.Icon;
            _unlockedImage.sprite = _currentItem.Icon;
            _canMakeImage.sprite = _currentItem.Icon;
        }
    }
    void RefreshIconImage()
    {
        if(!_unlocked)
        {
            _unlockedImage.enabled = false;
            _canMakeImage.enabled = false;
        }
        else if(_unlocked && !_canMake)
        {
            _unlockedImage.enabled = true;
            _canMakeImage.enabled = false;
        }
        else if(_canMake)
        {
            _canMakeImage.enabled = true;
        }
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
        if (!_canMake) return;


        // TODO: 요리 아이템 생성 후 인벤토리에 추가
    }
}
