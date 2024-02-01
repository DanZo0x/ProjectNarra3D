using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource _musicSource;
    public AudioSource _sfxSource;

    [Space]
    [Header("Audio Clips")]
    public AudioClip _menuMusic;
    public AudioClip _clickOnButton;
    public AudioClip _dialogueSfx;

    [Space]
    [Header("Audio References")]
    public static AudioManager _instance;
    public GameObject _musicSourceObj;
    public GameObject _sfxSourceObj;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _musicSourceObj = GameObject.FindGameObjectWithTag("MusicSource");
        _sfxSourceObj = GameObject.FindGameObjectWithTag("SfxSource");
        _musicSource = _musicSourceObj.GetComponent<AudioSource>();
        _sfxSource = _sfxSourceObj.GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    public void PlaySFX(AudioClip _clip)
    {
        _sfxSource.PlayOneShot(_clip);
    }
}
