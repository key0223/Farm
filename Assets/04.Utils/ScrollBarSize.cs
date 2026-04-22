using UnityEngine;
using UnityEngine.UI;

public class ScrollBarSize : MonoBehaviour
{
    Scrollbar _scrollbar;

    void Awake()
    {
        _scrollbar = GetComponent<Scrollbar>();
    }


    void Update()
    {
        _scrollbar.size = 0.05f;
    }
    
}
