using UnityEngine;
using static Define;

[RequireComponent(typeof(AudioSource))]
public class Sound :MonoBehaviour
{
    AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();   
    }

    public void SetSound(SoundItem item)
    {
        _audioSource.pitch = Random.Range(item.SoundPitchRandomVariationMin, item.SoundPitchRandomVariationMax);
        _audioSource.volume = item.SoundVolume;
        _audioSource.clip = item.SoundClip;
    }

    public void Play()
    {
        if (_audioSource.clip != null)
            _audioSource.Play();
    }
    //void OnEnable()
    //{
    //    if(_audioSource.clip != null)
    //        _audioSource.Play();
    //}

    void OnDisable()
    {
        _audioSource.Stop();
    }
}
