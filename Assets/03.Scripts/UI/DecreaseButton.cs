using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecreaseButton : HoldQuantityButton
{
    ShopPurchase _shopPurchase;

    protected override void Start()
    {
        base.Start();
        if (_shopPurchase == null)
            _shopPurchase = GetComponentInParent<ShopPurchase>();
    }

    protected override void OnSinglePress()
    {
        _shopPurchase?.DecreaseQuantity();
    }
    protected override void OnRepeatPress()
    {
        _shopPurchase?.DecreaseQuantity();
    }
}
