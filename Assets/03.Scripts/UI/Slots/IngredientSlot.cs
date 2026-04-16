using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Image _itemIconImage;
    [SerializeField] TextMeshProUGUI _itemCountText;
    [SerializeField] TextMeshProUGUI _itemNameText;

    public void SetUI(int itemId, int count)
    {
        ItemDataBase itemData;
        TableDataManager.Instance.ItemDict.TryGetValue(itemId, out itemData);
        if (itemData == null) return;

        Sprite icon = itemData.Icon;

        _itemIconImage.sprite = icon;
        _itemCountText.text = count.ToString();
        _itemNameText.text = LocalizationManager.Instance.GetString(itemData.DisplayName);
    }

    public void Clear()
    {
        _itemCountText.text = "";
        _itemNameText.text = "";
    }

}
