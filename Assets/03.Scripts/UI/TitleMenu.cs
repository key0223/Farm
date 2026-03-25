using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : ClickableMenu
{
    [SerializeField] Button _newGameButton;
    [SerializeField] DOTweenAnimation _tweenMain;
    [SerializeField] float _customizationX;
    [SerializeField] float _mainX;
    [SerializeField] float _loadX;

   
    protected override void Awake()
    {
        base.Awake();
        _menuName = "Title";
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);

        _newGameButton.onClick.AddListener(SlideToCustomMenu);
    }


    void SlideToCustomMenu()
    {

        if (_tweenMain.tween is Tweener tweener)
        {
            tweener.ChangeEndValue(new Vector3(_loadX, 0, 0));
            _tweenMain.DORestart(true);
        }
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
