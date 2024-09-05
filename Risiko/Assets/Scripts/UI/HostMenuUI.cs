using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class HostMenuUI : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private Button updateButton;
        [SerializeField] private Button runGameButton;
        [SerializeField] private TMP_Text playerList;
        [SerializeField] private TMP_Text lobbyID;
        [SerializeField] private GameObject popUpDiceHostMenu;
        [SerializeField] private Button addBotButton;
        [SerializeField] private Button removeBotButton;

        private readonly float _delay = 1.0f; // Durata del ritardo in secondi
        private float _timer;
        private string _playerListFromServer;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                ClientManager.Instance.KillLobby();
                Player.Instance.ResetPlayer();
                GameManager.Instance.ResetGameManager();

                SceneManager.LoadScene("GameMenu");
            });

            runGameButton.onClick.AddListener(() => {
                ClientManager.Instance.StartHostGame();
                popUpDiceHostMenu.SetActive(true);
            });

            updateButton.onClick.AddListener(() => { ClientManager.Instance.RequestNameUpdatePlayerList(); });
        }

        void Start() {
            _playerListFromServer = null;
            ClientManager.Instance.CreateLobbyAsHost();
            _timer = _delay;
        }

        private void Update() {
            lobbyID.text = GameManager.Instance.GetLobbyId();

            if (_timer > 0) {
                _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
            } else {
                ClientManager.Instance
                    .SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
                ClientManager.Instance.RequestNameUpdatePlayerList();

                // Reset del timer
                _timer = _delay;
                Debug.Log("HOSTMENU - playerList:" + playerList.text);
            }

            //Aggiornamento lista giocatori
            _playerListFromServer = string.Join(", ", GameManager.Instance.PlayersName);
            playerList.text = "Players: " + _playerListFromServer;


            //Quando i giocatori saranno 3+
            if (GameManager.Instance.GetPlayersNumber() >= 2) {
                runGameButton.interactable = true;
            } else {
                runGameButton.interactable = false;
            }
        }
    }
}