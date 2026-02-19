using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SingleAnimatedSprite : MonoBehaviour
{
    public event Action<string, int> OnAnimationFinished;

    SpriteRenderer _renderer;
    AnimationData _currentAnim;

    [SerializeField] float _interval = 0.1f;
    int _frameIndex;
    int _currentDirection;
    float _timer;
    Dictionary<string, AnimationData> _animationClipDict = new Dictionary<string, AnimationData>();

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateFlip();
        UpdateAnimation();
    }

    public void InitAnimationDict(string targetName)
    {
        Dictionary<string, AnimationDataBase> rawDict = TableDataManager.Instance.GetNPCAnimationDict(targetName);

        foreach(var kvp in rawDict)
        {
            AnimationDataBase data = kvp.Value;

            Sprite[] sprites = GetSprites(data.SheetDirectory, data.ParentSheet, data.SpriteIndex);

            AnimationData clip = new AnimationData()
            {
                LayerType = data.LayerType,
                Sprites = sprites,
                Loop = data.Loop,
            };

            _animationClipDict.Add(kvp.Key, clip);
        }
    }

    public void PlayAnim(string animName)
    {
        if (_animationClipDict.TryGetValue(animName, out AnimationData clip))
        {
            _currentAnim = clip;
            _frameIndex = 0;
            _timer = 0;
        }
    }

    void UpdateAnimation()
    {
        if (_currentAnim == null) return;

        _timer += Time.deltaTime;
        if(_timer >= _interval)
        {
            NextFrame();
            _timer = 0;
        }
    }

    void NextFrame()
    {
        _frameIndex++;
        if(_frameIndex>= _currentAnim.Sprites.Length)
        {
            if (_currentAnim.Loop)
                _frameIndex = 0;
            else
            {
                OnAnimationFinished?.Invoke(GetCurrentAnimName(), _currentDirection);
                return;
            }
        }

        if(_frameIndex < _currentAnim.Sprites.Length)
            _renderer.sprite = _currentAnim.Sprites[_frameIndex];
    }
    void UpdateFlip()
    {
        bool shouldFlip = (_currentDirection == 0);
        _renderer.flipX = shouldFlip;

    }
    public void SetDirection(int direction)
    {
        _currentDirection = Mathf.Clamp(direction, 0, 3);
    }
    #region Utils
    string GetCurrentAnimName()
    {
        foreach(var kvp in _animationClipDict)
        {
            if (kvp.Value == _currentAnim)
            {
                string actionName = kvp.Key;
                return actionName;
            }
        }
        return null;
    }

    Sprite[] GetSprites(string directory, string sheetName, List<int> indexes)
    {
        string basePath = $"{directory}/{sheetName}";

        Sprite[] all = Resources.LoadAll<Sprite>(basePath);
        Sprite[] sprites = new Sprite[indexes.Count];

        for (int i = 0; i < indexes.Count; i++)
        {
            int idx = indexes[i];
            if (idx >= 0 && idx < all.Length)
            {
                sprites[i] = all[idx];
            }
            else
            {
                Debug.LogWarning($"index {idx} out of range (total {all.Length} sprites)");
                sprites[i] = null;
            }
        }

        return sprites;
    }
    #endregion

}
