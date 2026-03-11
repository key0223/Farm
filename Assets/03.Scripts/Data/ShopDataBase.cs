using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopDataBase 
{
    public string ShopId;
    public string DisplayName;
    public string ShopOwner;
    public int OpenTime;
    public int CloseTime;
    public string DayOff;
    public string ClosedMessage;
    public string SalableCategoriesString;

    public string[] SalableCategories;

}
