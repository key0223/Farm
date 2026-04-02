using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class SlideButton : MonoBehaviour,IPointerEnterHandler
{
    [SerializeField] TitleMenu _titleMenu;
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _backText;
    [SerializeField] int _targetIndex;
    void Awake()
    {
        _button.onClick.AddListener(SlideToTarget);
        
    }

    void SlideToTarget()
    {
        SoundManager.Instance.PlaySound(SoundName.UI_CLICK_3);
       _titleMenu.SlideToIndex(_targetIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(SoundName.UI_HOVER_1);

    }
}
