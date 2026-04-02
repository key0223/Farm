using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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
        SoundManager.Instance.PlaySound(SoundName.UI_CLICK_2);
        _bgImage.gameObject.SetActive(true);
    }
    
}
