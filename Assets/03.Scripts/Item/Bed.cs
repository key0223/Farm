using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{

    bool _canGotoSleep = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(_canGotoSleep)
        {
            if (collision.gameObject.CompareTag("Player"))
                UIManager.Instance.ShowSave();

            _canGotoSleep = false;
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _canGotoSleep = true;
        }
    }
}
