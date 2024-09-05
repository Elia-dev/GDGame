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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void SetVolume(float volume)
        {
            gameTrack.volume = volume;
            menuTrack.volume = volume;
            winTrack.volume = volume;
            loseTrack.volume = volume;
        }
        /*public static BGMusic_selector Instance { get; private set; }

        public AudioSource menuTrack;
        public AudioSource gameTrack;
        public AudioSource winTrack;
        public AudioSource loseTrack;


        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }*/

        void Start() {
            menuTrack.Play();
            gameTrack.Stop();
            winTrack.Stop();
            loseTrack.Stop();
        }

        void Update() {
            if (SceneManager.GetActiveScene().name.Equals("MainMenu") && !menuTrack.isPlaying) {
                Debug.Log("Menu Pirati caraibi");
                gameTrack.Stop();
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Play();
            } else if (SceneManager.GetActiveScene().name.Equals("Main") && !gameTrack.isPlaying && !winTrack.isPlaying &&
                     !loseTrack.isPlaying) {
                Debug.Log("Gioco Mia si da foho");
                winTrack.Stop();
                loseTrack.Stop();
                menuTrack.Stop();
                gameTrack.Play();
            }
            else if (SceneManager.GetActiveScene().name.Equals("Main") &&
                     !GameManager.Instance.getWinnerGameId().Equals("")) {
                if (GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) && !winTrack.isPlaying) {
                    Debug.Log("Hai vinto scemo");
                    loseTrack.Stop();
                    menuTrack.Stop();
                    gameTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    winTrack.Play();
                }
                else if (!GameManager.Instance.getWinnerGameId().Equals(Player.Instance.PlayerId) &&
                         !loseTrack.isPlaying) {
                    Debug.Log("Hai perso coglione");
                    menuTrack.Stop();
                    gameTrack.Stop();
                    winTrack.Stop();
                    SFX_selector.Instance.StopAllSFX();
                    loseTrack.Play();
                }
            }
        }
    }
}