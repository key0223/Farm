using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResponseButton : ClickableComponent
{
    [SerializeField] TextMeshProUGUI _responseText;
    [SerializeField] Color _defaultColor;
    [SerializeField] Color _highlightColor;

    Action<int> _onResponseSelected;


    int _buttonIndex;
    bool _isHovered = false;

    public Action<int> OnResponseSelected { get { return _onResponseSelected; } set { _onResponseSelected = value; } }
    public int ButtonIndex { get { return _buttonIndex; } set { _buttonIndex = value; } }

   
    public void SetResponseText(string response,Action<int> callback)
    {
        _responseText.text = response;
        _responseText.color = _defaultColor;
        OnResponseSelected = callback;
    }

    public void SelectResponse()
    {
        _onResponseSelected?.Invoke(_buttonIndex);
    }

    public override void OnHover()
    {
        if (_isHovered) return;

        _isHovered = true;
        _responseText.color = _highlightColor;
    }
    public override void OnHoverExit()
    {
        if(!_isHovered) return;

        _isHovered = false;
        _responseText.color = _defaultColor;
    }
}
