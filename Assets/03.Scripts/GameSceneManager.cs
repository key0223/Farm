using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class GameSceneManager : SingletonMonobehaviour<GameSceneManager>
{
    public event Action OnAfterSceneLoad;
    public event Action OnBeforeSceneUnloadFadeOut;
    public event Action OnBeforeSceneUnload;
    public event Action OnAfterSceneLoadFadeIn;

    [SerializeField] SceneName _startScene;
    [SerializeField] CanvasGroup _faderCanvasGroup;
    [SerializeField] Image _faderImage;
    float _fadeDuration = 1;
    bool _isFading;

    public SceneName StartScene { get {  return _startScene; } }
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.ManagerReady("GameSceneManager");
    }

    IEnumerator Start()
    {
        _faderImage.color = new Color(0f,0f,0f,1f); /* Start off with a black color */
        _faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(CoLoadSceneAndSetActive(_startScene.ToString()));
        OnAfterSceneLoad?.Invoke();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(CoFade(0));
    }

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (!_isFading)
            StartCoroutine(CoFadeAndSwitchScene(sceneName, spawnPosition));
    }

    IEnumerator CoLoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }
    IEnumerator CoFadeAndSwitchScene(string sceneName, Vector3 spawnPosition)
    {
        OnBeforeSceneUnloadFadeOut?.Invoke();
        yield return StartCoroutine(CoFade(1f)); /* Fading to black */

        SaveLoadManager.Instance.StoreCurrentSceneData();

        PlayerController player = FindObjectOfType<PlayerController>();
        player.transform.position = spawnPosition;
        player.CellPos = GridUtils.WorldToGrid(player.transform.position);

        OnBeforeSceneUnload?.Invoke();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex); /* Unload current scene */

        yield return StartCoroutine(CoLoadSceneAndSetActive(sceneName)); /* Load new scene */
        MapManager.Instance.SetCurrentLocation(SceneManager.GetActiveScene().name);
        OnAfterSceneLoad?.Invoke();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(CoFade(0));
        OnAfterSceneLoadFadeIn?.Invoke();
    }

    IEnumerator CoFade(float finalAlpha)
    {
        _isFading = true;
        _faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(_faderCanvasGroup.alpha -  finalAlpha) / _fadeDuration;

        while(!Mathf.Approximately(_faderCanvasGroup.alpha,finalAlpha))
        {
            _faderCanvasGroup.alpha = Mathf.MoveTowards(_faderCanvasGroup.alpha,finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        _isFading = false;
        _faderCanvasGroup.blocksRaycasts = false;
    }
}
