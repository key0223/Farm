using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabContainer : ClickableMenu
{
    [SerializeField] List<ClickableMenu> _tabPages = new List<ClickableMenu>();
    [SerializeField] List<TabButton> _tabButtons = new List<TabButton>();

    protected override void Awake()
    {
        base.Awake();
        //_menuName = "TabContainer";
    }
    protected override void Start()
    {
        base.Start();
        UIManager.Instance.RegisterTabs(MenuName, _tabPages);
        _tabButtons[0].SetBGImage(true);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllManagersReady)
            return;
        UIManager.Instance.RegisterTabs(MenuName, _tabPages);

    }
    protected override void OnDisable()
    {
        base.OnDisable();

    }
    protected override void SubscribeEvent()
    {
        base.SubscribeEvent();
    }

    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        TabButton clicked = null;

        foreach (TabButton button in _tabButtons)
        {
            if (button.ContainsPoint((int)screenPos.x, (int)screenPos.y))
            {
                clicked = button;
                break;
            }
        }

        if (clicked != null)
        {
            foreach (TabButton button in _tabButtons)
            {
                button.SetBGImage(false);
            }

            int index = _tabButtons.IndexOf(clicked);
            clicked.OnLeftClick(screenPos);
            UIManager.Instance.ShowTab(_menuName, index);
        }
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //
    }

    protected override void Exit()
    {
        UIManager.Instance.HandleEscape();
    }

}
