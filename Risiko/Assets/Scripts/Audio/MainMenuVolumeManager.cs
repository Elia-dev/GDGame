using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuVolumeManager : MonoBehaviour
{
    public AudioSource musicSource;

    void Start()
    {
        Debug.Log("Musica in memoria: " + PlayerPrefs.GetFloat("MusicVolume"));
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
    }
}
