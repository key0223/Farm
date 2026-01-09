using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    [Header("Other Settings")]
    [SerializeField] float _musicTransitionSecs = 8f;

    Dictionary<SoundName, SoundItem> _soundDict = new Dictionary<SoundName, SoundItem>();

    Coroutine _coPlaySceneSounds;

    protected override void Awake()
    {
        base.Awake();
        //GameManager.OnAllManagersReady += SubscribeEvent;

        Init();
        GameManager.Instance.ManagerReady("SoundManager");
    }
    void SubscribeEvent()
    {
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void Init()
    {
        foreach(SoundItem item in _soundList.SoundDetails)
            _soundDict.Add(item.SoundName, item);
    }

    public void PlaySound(SoundName soundName)
    {
        if(_soundDict.TryGetValue(soundName, out SoundItem soundItem))
        {
            GameObject soundObj = ResourceManager.Instance.Instantiate("Sound");
            //GameObject soundObj = Instantiate(_soundPrefab,Vector3.zero, Quaternion.identity);

            Sound sound = soundObj.GetComponent<Sound>();
            sound.SetSound(soundItem);
            sound.Play();
            //soundObj.SetActive(true);
            StartCoroutine(CoDisableSound(soundObj,soundItem.SoundClip.length));

        }
    }

    IEnumerator CoDisableSound(GameObject soundObj,float duration)
    {
        yield return new WaitForSeconds(duration);
        ResourceManager.Instance.Destroy(soundObj);
    }
}
