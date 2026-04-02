using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class DecreaseButton : HoldQuantityButton
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
        SoundManager.Instance.PlaySound(SoundName.UI_CLICK_1);
        _target?.DecreaseQuantity();
    }
    protected override void OnRepeatPress()
    {
        _target?.DecreaseQuantity();
    }
}
