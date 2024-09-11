using System.Collections;
using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio {
    public class BGMusic_selector : MonoBehaviour {
        public static BGMusic_selector Instance { get; private set; }
        public AudioSource gameTrack;
        public AudioSource menuTrack;
        public AudioSource winTrack;
        public AudioSource loseTrack;
        public AudioSource easterEggTrack;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void SetVolume(float volume) {
            gameTrack.volume = volume;
            menuTrack.volume = volume;
            winTrack.volume = volume;
            loseTrack.volume = volume;
            easterEggTrack.volume = volume;
        }

        void Start() {
            menuTrack.Play();
            gameTrack.Stop();
            winTrack.Stop();
            loseTrack.Stop();
            easterEggTrack.Stop();
        }

        void Update() {
            if (SceneManager.GetActiveScene().name.Equals("MainMenu") && !menuTrack.isPlaying) {
                gameTrack.Stop();
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Play();
                easterEggTrack.Stop();
            } else if (SceneManager.GetActiveScene().name.Equals("Main") && !gameTrack.isPlaying && !winTrack.isPlaying &&
                      !loseTrack.isPlaying && !easterEggTrack.isPlaying) {
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Stop();
                gameTrack.Play();
                easterEggTrack.Stop();
            } else if (SceneManager.GetActiveScene().name.Equals("Main") &&
                      !GameManager.Instance.getWinnerGameId().Equals("")) {
                if (GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) && !winTrack.isPlaying &&
                    !easterEggTrack.isPlaying) {
                    loseTrack.Stop();
                    menuTrack.Stop();
                    gameTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    // Faccio suonare l'inno dell'URSS se il rosso vince
                    if (Player.Instance.ArmyColor.Equals("red")) {
                        easterEggTrack.Play();
                        winTrack.Stop();
                    }
                    else {
                        easterEggTrack.Stop();
                        winTrack.Play();
                    }
                } else if (!GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) &&
                          !loseTrack.isPlaying) {
                    menuTrack.Stop();
                    gameTrack.Stop();
                    winTrack.Stop();
                    easterEggTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    loseTrack.Play();
                }
            }
        }
    }
}