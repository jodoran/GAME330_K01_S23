using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingAudio : MonoBehaviour
{
    public bool MenuScene;
    private int AudioSet;

    private GameObject Music;
    private GameObject SoundEffects;

    private float MusicVolume;
    private float SFXVolume;

    // Start is called before the first frame update
    void Start()
    {
        Music = transform.GetChild(0).gameObject;
        SoundEffects = transform.GetChild(1).gameObject;
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        AudioSet = PlayerPrefs.GetInt("AudioToggle", 1);

        if (AudioSet == 0)
        {
            SetChildAudioSources(Music, 0);
            SetChildAudioSources(SoundEffects, 0);
        }
        else
        {
            SetChildAudioSources(Music, MusicVolume);
            SetChildAudioSources(SoundEffects, SFXVolume);
        }
    }

    private void FixedUpdate()
    {
        if (MenuScene)
        {
            if (PlayerPrefs.GetFloat("MusicVolume", 0.5f) != MusicVolume ||
            PlayerPrefs.GetFloat("SFXVolume", 0.5f) != SFXVolume ||
            PlayerPrefs.GetInt("AudioToggle", 1) != AudioSet)
            {
                Start();
            }
        }
    }

    void SetChildAudioSources(GameObject ParentObject, float _volume)
    {
        for (int i = 0; i <= ParentObject.transform.childCount - 1; i++)
        {
            AudioSource _audio = ParentObject.transform.GetChild(i).GetComponent<AudioSource>();
            _audio.volume = _volume;
        }
    }
}
