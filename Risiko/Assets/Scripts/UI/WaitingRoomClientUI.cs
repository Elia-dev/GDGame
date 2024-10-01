using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class WaitingRoomClientUI : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text playerList;
        [SerializeField] private TMP_Text lobbyID;
        [SerializeField] private GameObject popUpDice;

        private readonly float _delay = 1.0f; 
        private float _timer;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                ClientManager.Instance.LeaveLobby();
                GameManager.Instance.ResetGameManager();
                SceneManager.LoadScene("JoinGameMenu");
            });
        }

        void Start() {
            lobbyID.text = GameManager.Instance.GetLobbyId();
            _timer = _delay;
        }

        void Update() {
            if (_timer > 0) {
                _timer -= Time.deltaTime; 
            }
            else {
                ClientManager.Instance.RequestNameUpdatePlayerList();
                ClientManager.Instance.SendName(); 
                _timer = _delay;
            }

            string str = string.Join(", ", GameManager.Instance.PlayersName);
            playerList.text = "Players: " + str;

            if (!GameManager.Instance.GetGameWaitingToStart()) {
                popUpDice.SetActive(true);
            }
        }
    }
}
