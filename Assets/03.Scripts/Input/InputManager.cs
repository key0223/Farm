using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static Define;

public class InputManager : SingletonMonobehaviour<InputManager>
{
    public event Action OnInput;
    public event Action<Keys> OnKeyPressed; 
    public event Action OnEscapePressed;   

    InputState _input;
    readonly Keys[] _watchedKeys =
    {
        Keys.A,Keys.D,Keys.W,Keys.S,
        Keys.Left,Keys.Right,Keys.Up, Keys.Down,
        Keys.E,Keys.Escape,
        Keys.Alpha1,Keys.Alpha2,Keys.Alpha3,Keys.Alpha4,Keys.Alpha5,
        Keys.Alpha6,Keys.Alpha7,Keys.Alpha8,Keys.Alpha9,Keys.Alpha0,
    };

    // 기능 단위 버튼
    InputButton _useToolButton;
    InputButton _leftClickButton;

    public InputState InputState { get { return _input; } }
    protected override void Awake()
    {
        base.Awake();
        Init();

        GameManager.Instance.ManagerReady("InputManager");
    }

    void Init()
    {
        _input = new InputState();
        //_useToolButton = new InputButton(Keys.Space);
        _leftClickButton = new InputButton(MouseButtons.Left);
    }

    void Update()
    {
        _input.Update(_watchedKeys);
        OnInput?.Invoke();

        /* 키별 이벤트 */
        foreach (Keys key in _watchedKeys)
        {
            if (_input.IsNewKeyPress(key))
                OnKeyPressed?.Invoke(key);
        }
        if (_input.IsNewKeyPress(Keys.Escape))
            OnEscapePressed?.Invoke();


        if (_leftClickButton.JustPressed(_input))
        {
            Vector2 pos = _input.MousePosition;
            Debug.Log("Left Click Started");
        }
        if (_input.IsLeftHeld())
        {
            Vector2 pos = _input.MousePosition;
        }
        if (Mathf.Abs(_input.ScrollDelta) > 0.01f)
        {
            Debug.Log($"Scroll: {_input.ScrollDelta}");
        }
    }
  
    public static KeyCode ToKeyCode(Keys key)
    {
        switch(key)
        {
            case Keys.A : return KeyCode.A;
            case Keys.D : return KeyCode.D;
            case Keys.W : return KeyCode.W;
            case Keys.S : return KeyCode.S;
            case Keys.Left : return KeyCode.LeftArrow;
            case Keys.Right : return KeyCode.RightArrow;
            case Keys.Up : return KeyCode.UpArrow;
            case Keys.Down : return KeyCode.DownArrow;
            case Keys.E : return KeyCode.E;
            case Keys.Escape: return KeyCode.Escape;
            case Keys.Alpha1: return KeyCode.Alpha1;
            case Keys.Alpha2: return KeyCode.Alpha2;
            case Keys.Alpha3: return KeyCode.Alpha3;
            case Keys.Alpha4: return KeyCode.Alpha4;
            case Keys.Alpha5: return KeyCode.Alpha5;
            case Keys.Alpha6: return KeyCode.Alpha6;
            case Keys.Alpha7: return KeyCode.Alpha7;
            case Keys.Alpha8: return KeyCode.Alpha8;
            case Keys.Alpha9: return KeyCode.Alpha9;
            case Keys.Alpha0: return KeyCode.Alpha0;
            default: return KeyCode.None;
        }
    }
}
