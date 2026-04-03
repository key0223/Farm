using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : ClickableMenu
{
    protected override void Awake()
    {
        base.Awake();
        _menuName = "TestTab";
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!GameManager.Instance.AllManagersReady)
            return;

      
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

    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }
}
