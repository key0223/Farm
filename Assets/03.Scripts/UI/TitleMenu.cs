using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TitleMenu : ClickableMenu
{
    [SerializeField] float _slideDuration = 1.5f;
    [SerializeField] int _currentIndex;
    RectTransform _rectTransform;
    float[] _positions = { 1920, 0, -1920 };

    protected override void Awake()
    {
        base.Awake();
        _menuName = "Title";
        _rectTransform = GetComponent<RectTransform>();
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);

    }
    public void SlideToIndex(int index)
    {
        if (_rectTransform == null || index < 0 || index >= _positions.Length)
            return;

        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosX(_positions[index], _slideDuration,snapping:true)
                   .SetEase(Ease.Linear);
        _currentIndex = index;
    }
    #region Clickable
    public override void ReceiveLeftClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }

    public override void ReceiveRightClick(Vector2 screenPos)
    {
        //throw new System.NotImplementedException();
    }

    #endregion

}
