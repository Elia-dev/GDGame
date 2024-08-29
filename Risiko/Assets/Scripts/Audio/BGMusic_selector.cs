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
    public AudioSource conqueredTerritory;
    public AudioSource lostTerritory;
    private int i = 0;
    private int _countBattle = 0;
    void Start()
    {
        menuTrack.Play();
        gameTrack.Stop();
        winTrack.Stop();
        loseTrack.Stop();
        conqueredTerritory.Stop();
        lostTerritory.Stop();
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
            conqueredTerritory.Stop();
            lostTerritory.Stop();
            menuTrack.Play();
        }
        //getWinnerBattleId
        else if (SceneManager.GetActiveScene().name == "Main" && !gameTrack.isPlaying && !winTrack.isPlaying && !loseTrack.isPlaying)
        {
            Debug.Log("Main selected, GameTrack is not playing, ATTIVATA!");
            winTrack.Stop();
            loseTrack.Stop();
            menuTrack.Stop();
            conqueredTerritory.Stop();
            lostTerritory.Stop();
            gameTrack.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Main" && gameTrack.isPlaying && !conqueredTerritory.isPlaying &&
                 !lostTerritory.isPlaying)
        {
            if (_countBattle == 0)
            {
                if (!GameManager.Instance.getWinnerBattleId().Equals(""))
                {
                    if (GameManager.Instance.getWinnerBattleId().Equals(Player.Instance.PlayerId))
                    {
                        PlaySoundWithFade(conqueredTerritory);
                        _countBattle++;
                    }
                    else
                    {
                        PlaySoundWithFade(lostTerritory);
                        _countBattle++;
                    }
                }
            }

            if (GameManager.Instance.getWinnerBattleId().Equals(""))
            {
                _countBattle = 0;
            }
            
        }
        else if (SceneManager.GetActiveScene().name == "Main" && !GameManager.Instance.getWinnerGameId().Equals(""))
        {
            if (GameManager.Instance.getWinnerGameId() == Player.Instance.PlayerId && !winTrack.isPlaying)
            {
                Debug.Log("Main selected, I won the game, winTrack is not playing, ATTIVATA volte numero = " + i);
                loseTrack.Stop();
                menuTrack.Stop();
                gameTrack.Stop();
                conqueredTerritory.Stop();
                lostTerritory.Stop();
                winTrack.Play();
                Debug.Log("WinTrack.isPlaying = " + winTrack.isPlaying);
                i++;
            }
            else if(GameManager.Instance.getWinnerGameId() != Player.Instance.PlayerId && !loseTrack.isPlaying)
            {
                Debug.Log("Main selected, I Lose the game, loseTrack is not playing, ATTIVATA volte numero = " + i);
                menuTrack.Stop();
                gameTrack.Stop();
                winTrack.Stop();
                conqueredTerritory.Stop();
                lostTerritory.Stop();
                loseTrack.Play();
                Debug.Log("LoseTrack.isPlaying = " + loseTrack.isPlaying);
                i++;
            }
        }
    }

    private void PlaySoundWithFade(AudioSource sound)
    {
        StartCoroutine(FadeOutBackgroundMusic(sound));
    }

    private IEnumerator FadeOutBackgroundMusic(AudioSource sound)
    {
        float duration = 0.2f;
        float targetVolume = 0.05f;
        float startVolume = AudioListener.volume;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            gameTrack.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        gameTrack.volume = targetVolume;
        sound.Play();
        
        yield return new WaitForSeconds(sound.clip.length);
        
        StartCoroutine(FadeInBackgroundMusic(sound));
    }

    private IEnumerator FadeInBackgroundMusic(AudioSource sound)
    {
        float duration = 1.5f;
        float targetVolume = PlayerPrefs.GetFloat("musicVolume");
        float startVolume = gameTrack.volume;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            gameTrack.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        gameTrack.volume = targetVolume;
    }
}
