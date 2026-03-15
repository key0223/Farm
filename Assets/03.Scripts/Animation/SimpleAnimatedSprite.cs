using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleAnimatedSprite : MonoBehaviour
{
    [SerializeField] float _interval = 0.1f;
    [SerializeField] bool _isLoop = true;
    [SerializeField] List<Sprite> _sprites = new List<Sprite>();

    int _frameIndex = 0;
    float _timer = 0;

    SpriteRenderer _renderer;

    

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        _timer += Time.deltaTime;
        if (_timer >= _interval)
        {
            NextFrame();
            _timer = 0;
        }
    }
    void NextFrame()
    {
        _frameIndex++;
        if (_frameIndex >= _sprites.Count)
        {
            if (_isLoop)
                _frameIndex = 0;
        }

        if (_frameIndex < _sprites.Count)
            _renderer.sprite = _sprites[_frameIndex];
    }
}
