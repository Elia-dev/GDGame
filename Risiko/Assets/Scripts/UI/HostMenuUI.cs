using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class HostMenuUI : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private Button runGameButton;
        [SerializeField] private TMP_Text playerList;
        [SerializeField] private TMP_Text lobbyID;
        [SerializeField] private GameObject popUpDiceHostMenu;
        [SerializeField] private Button addBotButton;
        [SerializeField] private Button removeBotButton;

        private readonly float _delay = 1.0f; // Durata del ritardo in secondi
        private float _timer;
        private string _playerListFromServer;
        private int _botNumber = 0;
        private int _numPlayersOnAddBot = 0;
        private int _pendingBots = 0; // Numero di bot richiesti ma non ancora confermati
        private int _totalPlayers = 0;

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

            addBotButton.onClick.AddListener(() => {
                /*_botNumber++;
                _numPlayersOnAddBot = GameManager.Instance.GetPlayersNumber();
               // if(_botNumber + _numPlayersOnAddBot >= 6)
                addBotButton.interactable = false;
                await ClientManager.Instance.RequestAddBot();
                addBotButton.interactable = true;*/
                ClientManager.Instance.RequestAddBot();
                _pendingBots++; // Incrementa il contatore dei bot in attesa
                UpdateAddBotButtonState(); // Aggiorna lo stato del pulsante subito dopo la richiesta
            });

            removeBotButton.onClick.AddListener(() => {
                //_numPlayersOnAddBot = GameManager.Instance.GetPlayersNumber() - 1;
                /*if(_botNumber > 0)
                    _botNumber--;
                await ClientManager.Instance.RequestRemoveBot();*/
                if (_pendingBots > 0)
                    _pendingBots--; // Incrementa il contatore dei bot in attesa
                else
                    _totalPlayers = GameManager.Instance.GetPlayersNumber();
                UpdateAddBotButtonState(); // Aggiorna lo stato del pulsante subito dopo la richiesta
                ClientManager.Instance.RequestRemoveBot();
            });
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
            }

            //Aggiornamento lista giocatori
            _playerListFromServer = string.Join(", ", GameManager.Instance.PlayersName);
            playerList.text = "Players: " + _playerListFromServer;


            //Quando i giocatori saranno 3+
            runGameButton.interactable = GameManager.Instance.GetPlayersNumber() >= 2;

            // Aggiorna lo stato del pulsante addBot
            UpdateAddBotButtonState();
            
            /*if (GameManager.Instance.GetPlayersNumber() >= 6) 
                addBotButton.interactable = false;
            else {
                addBotButton.interactable = true;
            }
            /*
            else if (_numPlayersOnAddBot + _botNumber >= 6) {
                addBotButton.interactable = false;
            }
            else {
                addBotButton.interactable = true;
            } */
        }
        
        private void UpdateAddBotButtonState() {
            _totalPlayers = GameManager.Instance.GetPlayersNumber() + _pendingBots;

            // Se il numero totale di giocatori (inclusi i bot in attesa) è inferiore a 6, abilita il pulsante
            addBotButton.interactable = _totalPlayers < 6;

            // Calcola i bot in sospeso in base al numero effettivo di giocatori confermati
            if (_totalPlayers > GameManager.Instance.GetPlayersNumber()) {
                _pendingBots = _totalPlayers - GameManager.Instance.GetPlayersNumber();
            } else {
                _pendingBots = 0;
            }
        }
    }
}