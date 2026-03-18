using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPurchase : MonoBehaviour
{
    PlayerController _player;
    [SerializeField] TextMeshProUGUI _quantityText;
    [SerializeField] TextMeshProUGUI _totalPriceText;
    [SerializeField] Button _purchaseButton;

    int _itemPrice;
    int _totalPrice;

    int _minQuantity = 1;
    int _maxQuantity = 99;
    int _currentQuantity = 1;

    void Start()
    {
        _purchaseButton.onClick.AddListener(OnPurchase);
    }
    public void SetItemPrice(int price,PlayerController player)
    {
        _player = player;
        _itemPrice = price;
        Refresh();
    }
    public void IncreaseQuantity()
    {
        if (_currentQuantity < _maxQuantity)
        {
            _currentQuantity++;
            Refresh(); 
        }
    }

    public void DecreaseQuantity()
    {
        if (_currentQuantity > _minQuantity)
        {
            _currentQuantity--;
            _totalPrice = (_currentQuantity * _itemPrice);
            Refresh();
        }
    }

    void OnPurchase()
    {
        //UIManager.Instance.ShopUI.SelectedItem.Stack = _currentQuantity;
        //bool success = _player.PlayerInven.TryAdd(UIManager.Instance.ShopUI.SelectedItem);
        //if (success)
        //{
        //    Debug.Log("Purchase Succeed");
        //}
        //else
        //{
        //    Debug.Log("Purchase Failed");

        //}
    }
    void Refresh()
    {
        _totalPrice = (_currentQuantity * _itemPrice);
        _quantityText.text = _currentQuantity.ToString();
        _totalPriceText.text = _totalPrice.ToString();
    }
}
