using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Audio
{
    public class SFX_selector : MonoBehaviour
    {
        public static SFX_selector Instance { get; private set; }

        public AudioSource startedTurn;
        public AudioSource conqueredTerritory;
        public AudioSource lostTerritory;

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
            conqueredTerritory.Stop();
            lostTerritory.Stop();
            startedTurn.Stop();
        }

        public void StopAllSFX()
        {
            lostTerritory.Stop();
            conqueredTerritory.Stop();
            startedTurn.Stop();
        }

       /*
        void Update()
        {
            if (SceneManager.GetActiveScene().name.Equals("Main") && gameTrack.isPlaying &&
                !SFX_selector.Instance.conqueredTerritory.isPlaying &&
                !SFX_selector.Instance.lostTerritory.isPlaying)
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
        }
        */
    }
}