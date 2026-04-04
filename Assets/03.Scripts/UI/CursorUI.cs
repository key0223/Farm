using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CursorUI : MonoBehaviour
{
    [SerializeField] Image _cursorImage;
    
    Sprite _defaultCursorIcon;
    InputState _input;

    CursorType _currentCursorType;

    void Awake()
    {
        GameManager.OnAllManagersReady += Init;
    }

    void Init()
    {
        Cursor.visible = false;
        _input = InputManager.Instance.InputState;
        _defaultCursorIcon = ResourceManager.Instance.Load<Sprite>("Sprites/UI/UI2/cursor_02");
        GameManager.OnAllManagersReady -= Init;
    }
    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor()
    {
        _cursorImage.rectTransform.position = _input.MousePosition;
        ICursorProvider cursorProvider = GetCurrentCurosorProvider();

        if(cursorProvider != null )
            _currentCursorType = cursorProvider.GetCursorType();
        else
            _currentCursorType= CursorType.Default;

        SetCursorIcon();
    }

    ICursorProvider GetCurrentCurosorProvider()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(_input.MousePosition);
        //Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);

        if(hit.collider !=null)
        {
            ICursorProvider provider = hit.collider.GetComponent<ICursorProvider>();
            if(provider != null)
                return provider;
        }
        return null;
    }

    void SetCursorIcon()
    {
        switch(_currentCursorType )
        {
            case CursorType.Default:
                _cursorImage.sprite = _defaultCursorIcon;
                //Cursor.SetCursor(_defaultCursorIcon, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Pressed:
                break;
        }
    }
}
