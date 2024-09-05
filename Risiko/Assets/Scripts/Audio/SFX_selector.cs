using System.Collections;
using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Audio {
    public class SFX_selector : MonoBehaviour {
        public static SFX_selector Instance { get; private set; }

        public AudioSource startedTurn;
        public AudioSource conqueredTerritory;
        public AudioSource lostTerritory;
        private int _countBattle = 0;
        private bool _myTurnSoundActivated = false;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            /*else
            {
                Destroy(gameObject);
            }*/
        }

        void Start() {
            conqueredTerritory.Stop();
            lostTerritory.Stop();
            startedTurn.Stop();
        }

        public void StopAllSFX() {
            lostTerritory.Stop();
            conqueredTerritory.Stop();
            startedTurn.Stop();
        }


        void Update() {
            if (SceneManager.GetActiveScene().name.Equals("Main") && BGMusic_selector.Instance.gameTrack.isPlaying &&
                !conqueredTerritory.isPlaying &&
                !lostTerritory.isPlaying) {
                if (_countBattle == 0) {
                    if (!GameManager.Instance.getWinnerBattleId().Equals("")) {
                        if (GameManager.Instance.getWinnerBattleId().Equals(Player.Instance.PlayerId)) {
                            PlaySoundWithFade(BGMusic_selector.Instance.gameTrack, conqueredTerritory);
                            _countBattle++;
                        }
                        else {
                            PlaySoundWithFade(BGMusic_selector.Instance.gameTrack, lostTerritory);
                            _countBattle++;
                        }
                    }
                }

                if (GameManager.Instance.getWinnerBattleId().Equals("")) {
                    _countBattle = 0;
                }
            }

            if (SceneManager.GetActiveScene().name.Equals("Main") && !_myTurnSoundActivated &&
                Player.Instance.IsMyTurn) {
                PlaySoundWithFade(BGMusic_selector.Instance.gameTrack, startedTurn);
                _myTurnSoundActivated = true;
            }

            if (SceneManager.GetActiveScene().name.Equals("Main") && _myTurnSoundActivated &&
                !Player.Instance.IsMyTurn) {
                _myTurnSoundActivated = false;
            }
        }

        private void PlaySoundWithFade(AudioSource oldSound, AudioSource newSound) {
            StartCoroutine(FadeOutBackgroundMusic(oldSound, newSound));
        }

        private IEnumerator FadeOutBackgroundMusic(AudioSource oldSound, AudioSource newSound) {
            float duration = 0.2f;
            float targetVolume = 0.05f;
            float startVolume = AudioListener.volume;

            float time = 0;

            while (time < duration) {
                time += Time.deltaTime;
                oldSound.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                yield return null;
            }

            oldSound.volume = targetVolume;
            newSound.Play();

            yield return new WaitForSeconds(newSound.clip.length);

            StartCoroutine(FadeInBackgroundMusic(newSound, oldSound));
        }

        private IEnumerator FadeInBackgroundMusic(AudioSource oldSound, AudioSource newSound) {
            float duration = 1.2f;
            float targetVolume = PlayerPrefs.GetFloat("musicVolume");
            float startVolume = oldSound.volume;

            float time = 0;
            while (time < duration) {
                time += Time.deltaTime;
                newSound.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                yield return null;
            }

            newSound.volume = targetVolume;
        }
    }
}