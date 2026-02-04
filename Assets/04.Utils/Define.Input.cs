using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Define : MonoBehaviour
{
    public enum Keys
    {
       A,
       D,
       W,
       S,
       Left,
       Right,
       Up,
       Down,
       E,
       Escape,
       Return,

       /* Toolbar */
       Alpha1,
       Alpha2,
       Alpha3,
       Alpha4, 
       Alpha5,
       Alpha6, 
       Alpha7, 
       Alpha8, 
       Alpha9,
       Alpha0,
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
    }

    public enum ButtonState
    {
        Released,
        Pressed,
    }
}
