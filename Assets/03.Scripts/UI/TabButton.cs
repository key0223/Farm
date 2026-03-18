using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabButton : ClickableComponent
{
    [SerializeField] GameObject _bgImage;

    void Awake()
    {
        _bgImage.gameObject.SetActive(false);
    }
    public void SetBGImage(bool visible)
    {
        _bgImage.gameObject.SetActive(visible);
    }

    public override void OnLeftClick(Vector2 pos)
    {
        _bgImage.gameObject.SetActive(true);
    }
    
}
