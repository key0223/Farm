using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : ClickableMenu
{
    [SerializeField] TextMeshProUGUI _dialogueText;

    List<ResponseButton> _buttons = new List<ResponseButton>();

    
    public TextMeshProUGUI DialogueText { get {  return _dialogueText; }}
    public List<ResponseButton> Buttons { get { return _buttons; } }
    protected override void Awake()
    {
        base.Awake();
        _menuName = "Dialogue";
    }

    protected override void Start()
    {
        base.Start();
        InitButtons();

    }

    void InitButtons()
    {
        int index = 0;
        ResponseButton[] foundButtons = GetComponentsInChildren<ResponseButton>();
        for (int i = 0; i < foundButtons.Length; i++)
        {
            ResponseButton button = foundButtons[i];
            _buttons.Add(button);
            button.ButtonIndex = index;
            button.ClickableId = index;

            if(!_clickableComponents.Contains(button))
                _clickableComponents.Add(button);

            button.gameObject.SetActive(false);
            index++;
        }
    }

    public void SetButtonInactive()
    {
        foreach (ResponseButton button in _buttons)
        {
            button.gameObject.SetActive(false);
            button.OnResponseSelected = null;
        }
    }

    protected override void PerformHoverAction(Vector2 mousePos)
    {
        ClickableComponent previousHover = _currentClickableComponent;
        _currentClickableComponent = null;

        if (previousHover != null)
            previousHover.OnHoverExit();

        foreach (ClickableComponent component in _clickableComponents)
        {
            bool contains = component.ContainsPoint((int)mousePos.x, (int)mousePos.y);

            if (contains)
            {
                _currentClickableComponent = component;
                component.OnHover();

                return;
            }
        }
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
                ResponseButton button = component.GetComponent<ResponseButton>();
                if (button != null)
                    button.SelectResponse();
                break;
            }
        }
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }
}
