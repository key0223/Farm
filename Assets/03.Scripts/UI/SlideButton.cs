using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideButton : MonoBehaviour
{
    [SerializeField] TitleMenu _titleMenu;
    [SerializeField] Button _button;
    [SerializeField] int _targetIndex;
    void Awake()
    {
        _button.onClick.AddListener(SlideToTarget);
    }

    void SlideToTarget()
    {
       _titleMenu.SlideToIndex(_targetIndex);
    }
   
}
