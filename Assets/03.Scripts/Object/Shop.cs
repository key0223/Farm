using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] string _shopId;

    int _openTime;
    int _closeTime;
    string _dayOff;
    string _closedMessage;

    void Start()
    {
        InitShop();
    }
    void InitShop()
    {
        ShopDataBase data;
        TableDataManager.Instance.ShopDict.TryGetValue(_shopId, out data);
        if (data == null) return;

        _openTime = data.OpenTime;
        _closeTime = data.CloseTime;
        _dayOff = data.DayOff;
        _closedMessage = data.ClosedMessage;
    }
    public void Interact(PlayerController player)
    {
        UIManager.Instance.ShowShop(_shopId,player);
    }

}
