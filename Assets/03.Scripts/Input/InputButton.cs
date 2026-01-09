using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Define;

public class InputButton 
{
    public Keys? KeyboardKey { get; private set; }
    public MouseButtons? Mouse { get; private set; }

    public InputButton(Keys key)
    {
        KeyboardKey = key;
        Mouse = null;
    }
    public InputButton(MouseButtons button)
    {
        Mouse = button;
        KeyboardKey = null;
    }

    public bool JustPressed(InputState input)
    {
        if (KeyboardKey.HasValue && input.IsNewKeyPress(KeyboardKey.Value))
            return true;

        if (Mouse.HasValue)
        {
            switch (Mouse.Value)
            {
                case MouseButtons.Left:
                    return input.IsNewLeftClick();
                case MouseButtons.Right:
                    return input.IsNewRightClick();
            }
        }
        return false;
    }
    public bool Held(InputState input)
    {
        if (KeyboardKey.HasValue && input.IsKeyHeld(KeyboardKey.Value))
            return true;

        if (Mouse.HasValue && Mouse.Value == MouseButtons.Left)
            return input.IsLeftHeld();

        return false;
    }
}
