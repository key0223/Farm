using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] TextMeshProUGUI _itemTypeText;
    [SerializeField] TextMeshProUGUI _itemDescriptionText;

    [Header("Ingredient")]
    [SerializeField] GameObject _ingredientObj;
    [SerializeField] TextMeshProUGUI _ingredientText;

    string _ingredientPrefabPath = "UI/IngredientSlot";

    float _offsetX = 60f;
    float _offsetY = 20f;

    Canvas _overlayCanvas;
    RectTransform _rectTransform;
    RectTransform _canvasRect;

    List<IngredientSlot> _ingredientSlots = new List<IngredientSlot>();

    void Awake()
    {
        _overlayCanvas = GameObject.Find("Overlay Layer").GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasRect = _overlayCanvas.GetComponent<RectTransform>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
    void SetPosition(Vector2 mousePos)
    {
        //_rectTransform.pivot = new Vector2(0f, 0f);
        //Vector2 pos = mousePos + new Vector2(20f, 10f);

        //pos.x = Mathf.Clamp(pos.x, 10, Screen.width - 250);
        //pos.y = Mathf.Clamp(pos.y, 10, Screen.height - 100);

        //transform.position = pos;


        RectTransform canvasRect = _canvasRect;

        Camera uiCam = _overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _overlayCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePos,
            uiCam,
            out Vector2 localMousePos);

        Vector2 pivot = new Vector2(0f, 1f);
        _rectTransform.pivot = pivot;

        Vector2 size = _rectTransform.sizeDelta;

        float left = -canvasRect.rect.width * canvasRect.pivot.x;
        float right = canvasRect.rect.width * (1f - canvasRect.pivot.x);
        float top = canvasRect.rect.height * (1f - canvasRect.pivot.y);
        float bottom = -canvasRect.rect.height * canvasRect.pivot.y;

        float x = localMousePos.x + _offsetX;
        float y = localMousePos.y - _offsetY;

        x = Mathf.Clamp(x, left, right - size.x);
        y = Mathf.Clamp(y, bottom + size.y, top);

        _rectTransform.anchoredPosition = new Vector2(x, y);
    }
    public void Show(Item item, Vector2 mousePos, bool shouldHideText)
    {
        string name = LocalizationManager.Instance.GetString(item.DisplayName);
        string category = item.Category;
        string desc = LocalizationManager.Instance.GetString(item.Description);
        string color = item.CategoryColor;

        SetContentsByCategory(category, item,shouldHideText);

        if (shouldHideText)
        {
            name = ConvertToQuestionMark(name);
            category = ConvertToQuestionMark(category);
            desc = ConvertToQuestionMark(desc);
        }
        _itemNameText.text = name;
        _itemTypeText.text = LocalizationManager.Instance.GetString(category);
        _itemTypeText.color = Parser.ParseColor(color);
        _itemDescriptionText.text = desc;

        SetPosition(mousePos);

        gameObject.SetActive(true);
    }


    void SetContentsByCategory(string category, Item item,bool shouldHideText)
    {
        _ingredientObj.SetActive(false);

        switch (category)
        {
            case "Resource":
            case "Animal Product":
            case "Vegetable":
            case "Flower":
            case "Forage":
                break;
            case "Cooking":
                {
                    if (shouldHideText) return;

                    SetIngredientContents(item);
                    _ingredientObj.SetActive(true);
                }
                break;
        }
    }

    void SetIngredientContents(Item item)
    {
        int recipeId = TableDataManager.Instance.GetRecipeId(item.Id);

        RecipeDataBase recipeData;
        TableDataManager.Instance.RecipeDict.TryGetValue(recipeId, out recipeData);
        if (recipeData == null) return;

        foreach (Need need in recipeData.Needs)
        {
            GameObject slotObj = ResourceManager.Instance.Instantiate(_ingredientPrefabPath, _ingredientObj.transform);
            IngredientSlot slot = slotObj.GetComponent<IngredientSlot>();
            slot.SetUI(need.ItemId, need.Count);

            _ingredientSlots.Add(slot);
        }


    }
    public void Hide()
    {
        ClearIngredientSlots();
        gameObject.SetActive(false);
    }

    void ClearIngredientSlots()
    {
        for (int i = _ingredientSlots.Count - 1; i >= 0; i--)
        {
            _ingredientSlots[i].Clear();
            ResourceManager.Instance.Destroy(_ingredientSlots[i].gameObject);
        }

        _ingredientSlots.Clear();
    }
    string ConvertToQuestionMark(string str)
    {
        return new string('?', str.Length);
    }
}
