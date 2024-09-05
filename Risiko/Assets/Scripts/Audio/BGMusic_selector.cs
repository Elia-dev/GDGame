using System.Collections;
using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class BGMusic_selector : MonoBehaviour
    {
        public static BGMusic_selector Instance { get; private set; }

        public AudioSource menuTrack;
        public AudioSource gameTrack;
        public AudioSource winTrack;
        public AudioSource loseTrack;
        private int _countBattle = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            menuTrack.Play();
            gameTrack.Stop();
            winTrack.Stop();
            loseTrack.Stop();
        }

        void Update()
        {
            if (SceneManager.GetActiveScene().name.Equals("MainMenu") && !menuTrack.isPlaying)
            {
                gameTrack.Stop();
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Play();
            }
            else if (SceneManager.GetActiveScene().name.Equals("Main") && !gameTrack.isPlaying && !winTrack.isPlaying && !loseTrack.isPlaying)
            {
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Stop();
                gameTrack.Play();
            }
            else if (SceneManager.GetActiveScene().name.Equals("Main") && !GameManager.Instance.getWinnerGameId().Equals(""))
            {
                if (GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) && !winTrack.isPlaying)
                {
                    loseTrack.Stop();
                    menuTrack.Stop();
                    gameTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    winTrack.Play();
                }
                else if(!GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) && !loseTrack.isPlaying)
                {
                    menuTrack.Stop();
                    gameTrack.Stop();
                    winTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    loseTrack.Play();
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

            StartCoroutine(FadeInBackgroundMusic());
        }

        private IEnumerator FadeInBackgroundMusic()
        {
            float duration = 1.2f;
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
}