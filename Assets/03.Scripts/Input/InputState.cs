using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InputState 
{
    KeyboardState _currentKeyboard = new KeyboardState(Array.Empty<Keys>());
    KeyboardState _lastKeyboard = new KeyboardState(Array.Empty<Keys>());

    MouseState _currentMouse = MouseState.CaptureCurrent();
    MouseState _lastMouse = MouseState.CaptureCurrent();

    public void Update(IEnumerable<Keys> keysToWatch)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = KeyboardState.CaptureCurrent(keysToWatch);

        _lastMouse = _currentMouse;
        _currentMouse = MouseState.CaptureCurrent();
    }

    #region Keyboard Helper
    public bool IsNewKeyPress(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public bool IsKeyHeld(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyDown(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return _currentKeyboard.IsKeyUp(key) && _lastKeyboard.IsKeyDown(key);
    }

    #endregion

    #region Mouse Helper
    public bool IsNewLeftClick()
    {
        return _currentMouse.LeftButton == ButtonState.Pressed &&
               _lastMouse.LeftButton == ButtonState.Released;
    }

    public bool IsLeftHeld()
    {
        return _currentMouse.LeftButton == ButtonState.Pressed &&
               _lastMouse.LeftButton == ButtonState.Pressed;
    }

    public bool IsLeftReleased()
    {
        return _currentMouse.LeftButton == ButtonState.Released &&
               _lastMouse.LeftButton == ButtonState.Pressed;
    }

    public bool IsNewRightClick()
    {
        return _currentMouse.RightButton == ButtonState.Pressed &&
               _lastMouse.RightButton == ButtonState.Released;
    }

    public Vector2 MousePosition => new Vector2(_currentMouse.X, _currentMouse.Y);

    // 이번 프레임 스크롤 양 (위 +, 아래 -)
    public float ScrollDelta => _currentMouse.ScrollDelta;
    #endregion
}
