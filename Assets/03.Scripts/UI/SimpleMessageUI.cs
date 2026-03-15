using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleMessageUI : ClickableMenu
{
    [SerializeField] Canvas _canvas;
    [SerializeField] TextMeshProUGUI _messageText;

    protected override void Awake()
    {
        base.Awake();
        _menuName = "SimpleMessage";
    }

    public void Show(string message)
    {
        _messageText.text = message;
    }
    
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        ClickableComponent previousHover = _currentClickableComponent;
        _currentClickableComponent = null;

        foreach (ClickableComponent component in _clickableComponents)
        {
            bool contains = component.ContainsPoint((int)screenPos.x, (int)screenPos.y);

            if (contains)
            {
                _currentClickableComponent = component;
                _messageText.text = "";
                UIManager.Instance.HideSimpleMessage();
                break;
            }
        }

    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
      //  
    }
}
