using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AnimatedSprite : MonoBehaviour
{
    public event Action<string, int> OnAnimationFinished;

    [Header("Temp Settings")]
    [SerializeField] string _currentHair = "Culy";

    [SerializeField] SpriteRenderer _bodyRenderer;
    [SerializeField] SpriteRenderer _hairRenderer;
    [SerializeField] SpriteRenderer _armsRenderer;

    Dictionary<string, AnimationData> _animationClipDict = new Dictionary<string, AnimationData>();

    string[] _layersName = { "body", "hair", "arms" };
    SpriteRenderer[] _renderers;
    AnimationData[] _currentAnims; /* 부위별 현재 애니 */
    int[] _layerFrameIndexes;
    float[] _layerTimers;
    int _currentDirection = 0;
    float _interval = 0.1f;


    void Awake()
    {
        _renderers = new SpriteRenderer[] { _bodyRenderer, _hairRenderer, _armsRenderer };
        _currentAnims = new AnimationData[_renderers.Length];
        _layerFrameIndexes = new int[_renderers.Length];
        _layerTimers = new float[_renderers.Length];
    }
    void Start()
    {
        InitAnimationsFromTableData();
    }

    void Update()
    {
        UpdateFlipAllLayers();
        for (int i = 0; i < _renderers.Length; i++)
        {
            UpdateLayer(i);
        }
    }

    void UpdateLayer(int layerIndex)
    {
        if (_currentAnims[layerIndex] == null) return;

        AnimationData clip = _currentAnims[layerIndex];
        _layerTimers[layerIndex] += Time.deltaTime;


        if (_layerTimers[layerIndex] >= _interval)
        {
            NextFrame(layerIndex, clip);
            _layerTimers[layerIndex] = 0;
        }

    }

    void NextFrame(int layerIndex, AnimationData clip)
    {
        _layerFrameIndexes[layerIndex]++;

        if (_layerFrameIndexes[layerIndex] >= clip.Sprites.Length)
        {
            if (clip.Loop)
                _layerFrameIndexes[layerIndex] = 0;
            else
            {
                OnAnimationFinished?.Invoke(GetCurrentAnimName(layerIndex), _currentDirection);
                return;
            }
        }

        int frameIndex = _layerFrameIndexes[layerIndex];
        if (frameIndex < clip.Sprites.Length)
            _renderers[layerIndex].sprite = clip.Sprites[frameIndex];
    }
    void InitAnimationsFromTableData()
    {
        foreach (var kvp in TableDataManager.Instance.AnimationDict)
        {
            AnimationDataBase data = kvp.Value;

            if (data.LayerType == SpriteLayer.HAIR)
            {
                string actualSheet;
                if (!SheetMappingManager.Instance.GetActualSheet(data.ParentSheet, _currentHair, out actualSheet))
                    continue;
                else
                    data.ParentSheet = actualSheet;
            }

            Sprite[] sprites = GetSprites(data.SheetDirectory, data.ParentSheet, data.SpriteIndex);

            AnimationData clip = new AnimationData()
            {
                LayerType = data.LayerType,
                Sprites = sprites,
                Loop = data.Loop,
            };

            _animationClipDict.Add(data.AnimationName, clip);
        }
    }
    public void PlayAnimAllLayers(string action)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            string animName = $"{_layersName[i]}_{action}";
            PlayLayerAnim(i, animName);
            _layerTimers[i] = 0;
        }
    }

    public void PlayLayerAnim(int layerIndex, string animName)
    {
        if (_animationClipDict.TryGetValue(animName, out AnimationData clip))
            _currentAnims[layerIndex] = clip;

        _layerFrameIndexes[layerIndex] = 0;
        _layerTimers[layerIndex] = 0f;
    }

    public void PlayLayerAnimByType(int layerIndex, string action)
    {
        string animName = $"{_layersName[layerIndex]}_{action}";
        PlayLayerAnim(layerIndex, animName);
    }

    void UpdateFlipAllLayers()
    {
        bool shouldFlip = (_currentDirection == 0); /* Left일 때 Flip */

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null)
                _renderers[i].flipX = shouldFlip;
        }
    }

    public void SetDirection(int direction)
    {
        _currentDirection = Mathf.Clamp(direction, 0, 3);
    }
    #region Utils
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

    string GetCurrentAnimName(int layerIndex)
    {
        foreach (var kvp in _animationClipDict)
        {
            if (kvp.Value == _currentAnims[layerIndex])
            {
                string actionName = GetAnimActionName(kvp.Key);
                return actionName;
            }
        }
        return null;
    }

    string GetAnimActionName(string animName)
    {
        if (string.IsNullOrEmpty(animName)) return null;

        string[] parts = animName.Split('_');
        return parts.Length > 1 ? parts[^1] : animName; /* ^1 = parts[parts.Lenght-1] 와 같음 */
    }
    #endregion
}
