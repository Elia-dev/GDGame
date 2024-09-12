using System.Collections;
using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MatchmakingManager : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private GameObject popupError;
        [SerializeField] public GameObject rowPrefab; 
        [SerializeField] public Transform contentParent; 
        
        private static List<Lobby> _lobbies = new List<Lobby>();
        private readonly float _delay = 6.0f; 
        private float _timer;
        private bool _reloadLobbies = false;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                GameManager.Instance.ResetGameManager();
                Player.Instance.ResetPlayer();
                SceneManager.LoadScene("GameMenu");
            });
        }

        void Start() {
            _timer = _delay;
        
            _lobbies.ForEach(lobby => {
                GameObject newRow = Instantiate(rowPrefab, contentParent);
                newRow.transform.SetParent(contentParent);
                newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = "Lobby: " + lobby.GetLobbyID();
                newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = "Host: " + lobby.GetHostName();
                newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = "Players: " +lobby.GetPlayersNum();
                newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.GetLobbyID()));
            });
            rowPrefab.SetActive(false);
        }

        private void Update() {
            if (_timer > 0) {
                _timer -= Time.deltaTime;
            }

            if (_timer > 3 && !_reloadLobbies) {
                _reloadLobbies = true;
                rowPrefab.SetActive(true);
                foreach (Transform child in contentParent) {
                    Destroy(child.gameObject);
                }
                _lobbies.ForEach(lobby => {
                    GameObject newRow = Instantiate(rowPrefab, contentParent);
                    newRow.transform.SetParent(contentParent);
                    newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = "Lobby: " + lobby.GetLobbyID();
                    newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = "Host: " + lobby.GetHostName();
                    newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = "Players: " +lobby.GetPlayersNum();

                    newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.GetLobbyID()));
                });
                rowPrefab.SetActive(false);
            }
            if (_timer <= 0) {
                _timer = _delay;
                ClientManager.Instance.RequestAllGames();
                _reloadLobbies = false;
            }
        }

        public static void LoadAvailableLobbies(List<Lobby> availableLobbies) {
            _lobbies = availableLobbies;
        }

        private void JoinLobby(string idLobby) {
            GameManager.Instance.SetLobbyId(idLobby);
            ClientManager.Instance.JoinLobbyAsClient(idLobby);
            popupError.SetActive(true);
            GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                .SetErrorText("Trying to join the lobby");
            StartCoroutine(AttemptJoinLobby());
        }

        private IEnumerator AttemptJoinLobby() {
            float timerConnection = 5.0f;
            while (timerConnection > 0) {
                timerConnection -= Time.deltaTime;
                if (ClientManager.Instance.IsConnectedToLobby()) {
                    SceneManager.LoadScene("WaitingRoomClient");
                    yield break;
                }

                yield return null; 
            }

            GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                .SetErrorText("Unable to join the lobby.\nTry another one.");
        }
    }
}