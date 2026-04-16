using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Image _itemIconImage;
    [SerializeField] TextMeshProUGUI _itemCountText;
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] Color _defaultColor;
    [SerializeField] Color _notEnoughColor;
    public void SetUI(int itemId, int count)
    {
        ItemDataBase itemData;
        TableDataManager.Instance.ItemDict.TryGetValue(itemId, out itemData);
        if (itemData == null) return;

        Sprite icon = itemData.Icon;

        _itemIconImage.sprite = icon;
        _itemCountText.text = count.ToString();
        _itemNameText.text = LocalizationManager.Instance.GetString(itemData.DisplayName);

        int currentStack = GameManager.Instance.Player.PlayerInven.PlayerContainer.TryGetItemStack(itemId);

        if (currentStack < count)
            _itemNameText.color = _notEnoughColor;
    }

    public void Clear()
    {
        _itemCountText.text = "";
        _itemNameText.text = "";
        _itemNameText.color = _defaultColor;
    }

}
