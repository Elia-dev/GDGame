using System.Collections;
using System.Threading.Tasks;
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
        private bool _isBotAdded = false;
        private int _oldPlayerNumber = 0;
        
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

            addBotButton.onClick.AddListener(async() => {
                if (_botNumber < 5 && !_isBotAdded) {
                    _oldPlayerNumber = GameManager.Instance.GetPlayersNumber();
                    _isBotAdded = true;
                    _botNumber++;
                    //_numPlayersOnAddBot = GameManager.Instance.GetPlayersNumber();
                    // if(_botNumber + _numPlayersOnAddBot >= 6)
                    addBotButton.interactable = false;
                    removeBotButton.interactable = false;
                    await ClientManager.Instance.RequestAddBot();
                    await ClientManager.Instance.RequestBotNumber();
                    StartCoroutine(CheckPlayerNumber());
                    //addBotButton.interactable = true;
                    //_isBotAdded = false;
                }
            });

            removeBotButton.onClick.AddListener(async () => {
                //_numPlayersOnAddBot = GameManager.Instance.GetPlayersNumber() - 1;
                if(_botNumber > 0)
                    _botNumber--;
                /*if (_pendingBots > 0)
                    _pendingBots--; // Incrementa il contatore dei bot in attesa
                else*/
                    //_totalPlayers = GameManager.Instance.GetPlayersNumber();
                await ClientManager.Instance.RequestRemoveBot();
                await ClientManager.Instance.RequestBotNumber();
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
            
            /*if(GameManager.Instance.GetPlayersNumber() < 6) {
                Debug.Log("_botNumber: " + _botNumber + " serverBotNumber: " + GameManager.Instance.GetBotNumber());
                if (_botNumber == GameManager.Instance.GetBotNumber())
                    addBotButton.interactable = true;
            }
            else {
                addBotButton.interactable = false;
            }*/
            
            if (GameManager.Instance.GetPlayersNumber() >= 6) 
                addBotButton.interactable = false;
            else if (!_isBotAdded) {
                Debug.Log("_botNumber: " + _botNumber + " serverBotNumber: " + GameManager.Instance.GetBotNumber());
                if (_botNumber == GameManager.Instance.GetBotNumber())
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
        
        private IEnumerator CheckPlayerNumber() {
            while (GameManager.Instance.GetPlayersNumber() < _oldPlayerNumber + 1) {
                yield return new WaitForSeconds(0.1f);
            }
            _isBotAdded = false;
            removeBotButton.interactable = true;
            yield return null;
        }
    }
}