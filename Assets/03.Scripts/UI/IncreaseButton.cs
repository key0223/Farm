using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IncreaseButton : HoldQuantityButton
{
    [SerializeField] GameObject _targetReference;
    IQuantityAdjuster _target;

    protected override void Start()
    {
        base.Start();
        if (_target == null)
            _target = _targetReference.gameObject.GetComponent<IQuantityAdjuster>();
    }

    protected override void OnSinglePress()
    {
        _target?.IncreaseQuantity();
    }
    protected override void OnRepeatPress()
    {
        _target?.IncreaseQuantity();
    }
}
