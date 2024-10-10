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

        private readonly float _delay = 1.0f;
        private float _timer;
        private string _playerListFromServer;
        private int _botNumber = 0;
        private bool _isBotAdded = false;
        private int _oldPlayerNumber = 0;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                ClientManager.Instance.KillLobby();
                Player.Instance.ResetPlayer();
                GameManager.Instance.ResetGameManager();
                SceneManager.LoadScene("GameMenu");
            });

            runGameButton.onClick.AddListener(() => {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                ClientManager.Instance.StartHostGame();
                popUpDiceHostMenu.SetActive(true);
            });

            addBotButton.onClick.AddListener(async() => {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                if (_botNumber < 5 && !_isBotAdded) {
                    _oldPlayerNumber = GameManager.Instance.GetPlayersNumber();
                    _isBotAdded = true;
                    _botNumber++;
                    addBotButton.interactable = false;
                    removeBotButton.interactable = false;
                    await ClientManager.Instance.RequestAddBot();
                    await ClientManager.Instance.RequestBotNumber();
                    StartCoroutine(CheckPlayerNumber());

                }
            });

            removeBotButton.onClick.AddListener(async () => {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                if(_botNumber > 0)
                    _botNumber--;
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
                _timer -= Time.deltaTime;
            } else {
                ClientManager.Instance.SendName();
                ClientManager.Instance.RequestNameUpdatePlayerList();
                _timer = _delay;
            }

            _playerListFromServer = string.Join(", ", GameManager.Instance.PlayersName);
            playerList.text = "Players: " + _playerListFromServer;


            runGameButton.interactable = GameManager.Instance.GetPlayersNumber() >= 2;

            if (GameManager.Instance.GetPlayersNumber() >= 6)
                addBotButton.interactable = false;
            else if (!_isBotAdded) {
                //Debug.Log("_botNumber: " + _botNumber + " serverBotNumber: " + GameManager.Instance.GetBotNumber());
                if (_botNumber == GameManager.Instance.GetBotNumber())
                    addBotButton.interactable = true;
            }
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
