using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveUI : ClickableMenu
{
    [SerializeField] TextMeshProUGUI _gotoSleepText;
    [SerializeField] TextMeshProUGUI _yesText;
    [SerializeField] TextMeshProUGUI _noText;
    protected override void Awake()
    {
        base.Awake();
        _menuName = "Save";
    }

    protected override void Start()
    {
        base.Start();
    }
    #region Clickable
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
                SaveButton button = component.GetComponent<SaveButton>();
                if (button != null)
                    button.OnLeftClick(screenPos);
                break;
            }
        }
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }
    #endregion
}
