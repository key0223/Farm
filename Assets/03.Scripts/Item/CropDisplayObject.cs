using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDisplayObject : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void UpdateDisplay(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }
}
