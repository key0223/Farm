using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ClickableMenu : MonoBehaviour
{
    [Header("Menu Settings")]
    protected string _menuName;
    [SerializeField] protected int _menuId;
    [SerializeField] protected bool _showCloseButton = false;
    [SerializeField] protected GameObject _closeButton;

    [Header("Input Priority")]
    [SerializeField] protected bool _isAlwaysActive = false;

    protected InputState _input;
    protected List<ClickableComponent> _clickableComponents = new List<ClickableComponent>();
    protected ClickableComponent _currentClickableComponent;

    public int MenuId { get { return _menuId; } }
    public string MenuName { get { return _menuName; } }
    protected virtual void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
        SetUI();
    }

    protected virtual void Start()
    {
        _input = InputManager.Instance.InputState;
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;
        InputManager.Instance.OnInput -= HandleInput;
        InputManager.Instance.OnInput += HandleInput;
    }

    protected virtual void OnDisable()
    {
        InputManager.Instance.OnInput -= HandleInput;
    }
    protected virtual void SubscribeEvent()
    {
        InputManager.Instance.OnInput += HandleInput;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void SetUI()
    {
        if (_showCloseButton)
            ShowCloseButton();
    }

    public virtual bool CanReceiveInput()
    {
        return gameObject.activeInHierarchy && _isAlwaysActive;
    }
    public abstract void ReceiveLeftClick(Vector2 screenPos);
    public abstract void ReceiveRightClick(Vector2 screenPos);
    public virtual void LeftClickHeld(Vector2 screenPos) { }
    public virtual void ReceiveScrollWheel(float delta) { }
    public virtual void ReceiveKeyDown(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Escape && ShouldExitOnEscapeKey())
            Exit();
    }

    public virtual bool ShouldExitOnEscapeKey()
    {
        return true;
    }
    protected virtual void Exit()
    {
        gameObject.SetActive(false);
    }

    public void HandleInput()
    {
        if (_input == null) return;
        Vector2 mousePos = _input.MousePosition;

        PerformHoverAction(mousePos);

        if (_input.IsNewLeftClick())
            ReceiveLeftClick(mousePos);
        if (_input.IsNewRightClick())
            ReceiveRightClick(mousePos);

        if (_input.IsLeftHeld())
            LeftClickHeld(mousePos);

        if(_input.ScrollDelta != 0)
            ReceiveScrollWheel(_input.ScrollDelta);
    }

    protected virtual void PerformHoverAction(Vector2 mousePos)
    {
        ClickableComponent previousHover = _currentClickableComponent;
        _currentClickableComponent = null;

        foreach (ClickableComponent component in _clickableComponents)
        {
            if (component.ContainsPoint((int)mousePos.x, (int)mousePos.y))
            {
                _currentClickableComponent = component;
                component.OnHover();

                /* Tooltip UI */

                ContainerSlot slot = component.GetComponent<ContainerSlot>();
                if(slot != null && slot.Currentitem != null)
                {
                    string name = LocalizationManager.Instance.GetString(slot.Currentitem.DisplayName);
                    string itemType = slot.Currentitem.Category;
                    string desc = LocalizationManager.Instance.GetString(slot.Currentitem.Description);
                    string color = slot.Currentitem.CategoryColor;
                    UIManager.Instance.ShowTooltip(name, itemType,color, desc,mousePos);
                }
                break;
            }
        }
    }

    public virtual void PopulateClickableComponentList()
    {
        _clickableComponents.Clear();
        _clickableComponents.AddRange(GetComponentsInChildren<ClickableComponent>());
    }

    void ShowCloseButton()
    {
        Button closeButton = _closeButton.GetComponentInChildren<Button>();
        closeButton.onClick.AddListener(Exit);

        _closeButton.gameObject.SetActive(true);
    }
}
