using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BGMusic_selector : MonoBehaviour
{
    public AudioSource menuTrack;
    public AudioSource gameTrack;
    public AudioSource winTrack;
    public AudioSource loseTrack;
    void Start()
    {
        menuTrack.Play();
        gameTrack.Stop();
        winTrack.Stop();
        loseTrack.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update di volumeManager");
        if (SceneManager.GetActiveScene().name == "MainMenu" && !menuTrack.isPlaying)
        {
            Debug.Log("MainMenu selected, menuTrack is not playing, ATTIVATA!");
            gameTrack.Stop();
            winTrack.Stop();
            loseTrack.Stop();
            menuTrack.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Main" && !gameTrack.isPlaying)
        {
            Debug.Log("Main selected, GameTrack is not playing, ATTIVATA!");
            winTrack.Stop();
            loseTrack.Stop();
            menuTrack.Stop();
            gameTrack.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Main" && GameManager.Instance.getWinnerGameId() != "")
        {
            if (GameManager.Instance.getWinnerGameId() == Player.Instance.PlayerId && !winTrack.isPlaying)
            {
                Debug.Log("Main selected, I won the game, winTrack is not playing, ATTIVATA!");
                loseTrack.Stop();
                menuTrack.Stop();
                gameTrack.Stop();
                winTrack.Play();
                Debug.Log("WinTrack.isPlaying = " + winTrack.isPlaying);
            }
            else if(GameManager.Instance.getWinnerGameId() != Player.Instance.PlayerId && !loseTrack.isPlaying)
            {
                Debug.Log("Main selected, I Lose the game, loseTrack is not playing, ATTIVATA!");
                menuTrack.Stop();
                gameTrack.Stop();
                winTrack.Stop();
                loseTrack.Play();
                Debug.Log("LoseTrack.isPlaying = " + loseTrack.isPlaying);
            }
        }
    }
}
