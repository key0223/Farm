using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour,IInteractable
{
    public void Interact(PlayerController who)
    {
        UIManager.Instance.ShowCooking();
    }

}
