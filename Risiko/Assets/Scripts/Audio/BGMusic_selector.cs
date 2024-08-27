using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusic_selector : MonoBehaviour
{
    public AudioSource MenuTrack;
    public AudioSource GameTrack;
    public AudioSource WinTrack;
    public AudioSource LoseTrack;
    void Start()
    {
        MenuTrack.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" && !MenuTrack.isPlaying)
        {
            GameTrack.Stop();
            WinTrack.Stop();
            LoseTrack.Stop();
            MenuTrack.Play();
        }
        else if (SceneManager.GetActiveScene().name == "GameMenu" && !GameTrack.isPlaying)
        {
            WinTrack.Stop();
            LoseTrack.Stop();
            MenuTrack.Stop();
            GameTrack.Play();
        }
    }
}
