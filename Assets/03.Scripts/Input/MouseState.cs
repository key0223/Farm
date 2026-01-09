using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MouseState 
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public ButtonState LeftButton { get; private set; }
    public ButtonState RightButton { get; private  set; }
    public ButtonState MiddleButton { get; private set; }

    public float ScrollDelta { get; private set; }

    public MouseState(int x, int y, ButtonState left, ButtonState right, ButtonState middle, float scrollDelta)
    {
        X = x;
        Y = y;
        LeftButton = left;
        RightButton = right;
        MiddleButton = middle;
        ScrollDelta = scrollDelta;
    }

    public static MouseState CaptureCurrent()
    {
        Vector3 pos = Input.mousePosition;

        ButtonState left = Input.GetMouseButton(0) ? ButtonState.Pressed : ButtonState.Released;
        ButtonState right = Input.GetMouseButton(1) ? ButtonState.Pressed : ButtonState.Released;
        ButtonState middle = Input.GetMouseButton(2) ? ButtonState.Pressed : ButtonState.Released;

        float scrollDelta = Input.mouseScrollDelta.y;

        return new MouseState(
            (int)pos.x,
            (int)pos.y,
            left,
            right,
            middle,
            scrollDelta
        );
    }
}

