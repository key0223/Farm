using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class HoldQuantityButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] protected Color _normalColor = Color.white;
    [SerializeField] protected Color _pressColor = Color.gray;
    [SerializeField] protected float _holdDelay = 1.0f;
    [SerializeField] protected float _repeatRate = 0.2f;

    protected Image _buttonImage;

    protected bool _isHolding;
    protected bool _isRepeating;
    protected float _holdTimer;

    protected virtual void Start()
    {
        _buttonImage = GetComponent<Image>();
    }
    protected virtual void Update()
    {
        if (!_isHolding) return;

        _holdTimer += Time.unscaledDeltaTime;

        if (!_isRepeating && _holdTimer >= _holdDelay)
            _isRepeating = true;

        if (_isRepeating)
        {
            _holdTimer += Time.unscaledDeltaTime;

            if (_holdTimer >= _holdDelay + _repeatRate)
            {
                _holdTimer = _holdDelay;
                OnRepeatPress();
            }
        }
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _isHolding = true;
        _holdTimer = 0f;
        _isRepeating = false;
        _buttonImage.color = _pressColor;

        OnSinglePress();

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
        _isRepeating = false;
        _buttonImage.color = _normalColor;
    }

    protected abstract void OnSinglePress();
    protected abstract void OnRepeatPress();
}
