using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Define;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{

    [Header("Audio Sources")]
    [SerializeField] AudioSource _ambientSoundSource;
    [SerializeField] AudioSource _gameMusicSource;

    [Header("Audio Mixers")]
    [SerializeField] AudioMixer _gameAudioMixer;

    [Header("Audio Snapshots")]
    [SerializeField] AudioMixerSnapshot _gameMusicSnapshot;
    [SerializeField] AudioMixerSnapshot _gameAmbientSnapshot;

    [Header("Sound List")]
    [SerializeField] SO_SoundList _soundList;
    [SerializeField] SO_SceneSoundList _sceneSoundList;

    [Header("Other Settings")]
    [SerializeField] float _musicTransitionSecs = 8f;
    [SerializeField] float _defaultSceneMusicPlayTime = 120f;
    [SerializeField] float _sceneMusicStartMinSecs = 20f;
    [SerializeField] float _sceneMusicStartMaxSecs = 40f;

    Dictionary<SoundName, SoundItem> _soundDict = new Dictionary<SoundName, SoundItem>();
    Dictionary<SceneName, SceneSoundItem> _sceneSoundDict = new Dictionary<SceneName, SceneSoundItem>();

    Coroutine _coPlaySceneSounds;

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        Init();
        GameManager.Instance.ManagerReady("SoundManager");
    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;
        GameSceneManager.Instance.OnAfterSceneLoad -= PlaySceneSound;
        GameSceneManager.Instance.OnAfterSceneLoad += PlaySceneSound;
    }
    void OnDisable()
    {
        GameSceneManager.Instance.OnAfterSceneLoad -= PlaySceneSound;

    }
    void SubscribeEvent()
    {
        GameSceneManager.Instance.OnAfterSceneLoad += PlaySceneSound;

        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void Init()
    {
        foreach (SoundItem item in _soundList.SoundDetails)
            _soundDict.Add(item.SoundName, item);

        foreach (SceneSoundItem item in _sceneSoundList.SceneSoundDetails)
            _sceneSoundDict.Add(item.SceneName, item);
    }

    public void PlaySound(SoundName soundName)
    {
        if (_soundDict.TryGetValue(soundName, out SoundItem soundItem))
        {
            GameObject soundObj = ResourceManager.Instance.Instantiate("Sound");
            //GameObject soundObj = Instantiate(_soundPrefab,Vector3.zero, Quaternion.identity);

            Sound sound = soundObj.GetComponent<Sound>();
            sound.SetSound(soundItem);
            sound.Play();
            //soundObj.SetActive(true);
            StartCoroutine(CoDisableSound(soundObj, soundItem.SoundClip.length));

        }
    }

    void PlaySceneSound()
    {
        SoundItem musicSound = null;
        SoundItem ambientSound = null;

        float musicPlayTime = _defaultSceneMusicPlayTime;

        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            if (_sceneSoundDict.TryGetValue(currentSceneName, out SceneSoundItem sceneSoundItem))
            {
                _soundDict.TryGetValue(sceneSoundItem.MusicForScene, out musicSound);
                _soundDict.TryGetValue(sceneSoundItem.AmbientSoundForScene, out ambientSound);
            }
            else return;

            if (_coPlaySceneSounds != null)
                StopCoroutine(_coPlaySceneSounds);

            _coPlaySceneSounds = StartCoroutine(CoPlaySceneSounds(musicPlayTime, musicSound, ambientSound));
        }

    }

    void PlayAmbientSoundClip(SoundItem ambientSound, float transitionTime)
    {
        _gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSound.SoundVolume));

        _ambientSoundSource.clip = ambientSound.SoundClip;
        _ambientSoundSource.Play();

        _gameAmbientSnapshot.TransitionTo(transitionTime);
    }

    void PlayMusicSoundClip(SoundItem musicSound, float transitionTime)
    {
        _gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSound.SoundVolume));

        _gameMusicSource.clip = musicSound.SoundClip;
        _gameMusicSource.Play();
        _gameMusicSnapshot.TransitionTo(transitionTime);
    }

    float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        return (volumeDecimalFraction * 100f - 80f);
    }
    IEnumerator CoPlaySceneSounds(float musicPlayTime, SoundItem musicSound, SoundItem ambientSound)
    {
        if (musicSound != null && ambientSound != null)
        {
            PlayAmbientSoundClip(ambientSound, 0);

            yield return new WaitForSeconds(UnityEngine.Random.Range(_sceneMusicStartMinSecs, _sceneMusicStartMaxSecs));

            PlayMusicSoundClip(musicSound, _musicTransitionSecs);

            yield return new WaitForSeconds(musicPlayTime);

            PlayAmbientSoundClip(ambientSound, _musicTransitionSecs);
        }
    }
    IEnumerator CoDisableSound(GameObject soundObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        ResourceManager.Instance.Destroy(soundObj);
    }
}
