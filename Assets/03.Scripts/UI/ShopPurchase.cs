using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPurchase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _quantityText;
    [SerializeField] TextMeshProUGUI _totalPriceText;

    int _minQuantity = 1;
    int _maxQuantity = 99;
    int _currentQuantity = 1;

    
    public void IncreaseQuantity()
    {
        if (_currentQuantity < _maxQuantity)
        {
            _currentQuantity++;
            UpdateQuantityUI(); 
        }
    }

    public void DecreaseQuantity()
    {
        if (_currentQuantity > _minQuantity)
        {
            _currentQuantity--;
            UpdateQuantityUI();
        }
    }

    void UpdateQuantityUI()
    {
         _quantityText.text = _currentQuantity.ToString();
    }
}
