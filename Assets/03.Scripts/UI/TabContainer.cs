using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabContainer : ClickableMenu
{
    [SerializeField] List<ClickableMenu> _tabPages = new List<ClickableMenu>();
    [SerializeField] List<ClickableComponent> _tabButtons = new List<ClickableComponent>();

    protected override void Awake()
    {
        base.Awake();
        //_menuName = "TabContainer";
    }
    protected override void Start()
    {
        base.Start();
        UIManager.Instance.RegisterTabs(MenuName, _tabPages);
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

        for (int i = 0; i < _tabButtons.Count; i++)
        {
            if (_tabButtons[i].ContainsPoint((int)screenPos.x,(int)screenPos.y))
            {
                UIManager.Instance.ShowTab(_menuName,i);
                return;
            }
        }
        
        //ClickableMenu activeTab= UIManager.Instance.GetActiveTab(_menuName);
        //if (activeTab != null)
        //    activeTab.ReceiveLeftClick(screenPos);
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //
    }

}
