using System.Collections;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class JoinGameMenuUI : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] private Button browseLobbiesButton;
        [SerializeField] private TMP_InputField lobbyIdInputField;
        [SerializeField] private GameObject popUpIdLobbyError;
        private string _lobbyID;

        private void Awake() {
            ClientManager.Instance.RequestAllGames();
            backButton.onClick.AddListener(() => {
                GameManager.Instance.ResetGameManager();
                Player.Instance.ResetPlayer();
                SceneManager.LoadScene("GameMenu");
            });

            joinLobbyButton.onClick.AddListener(() => { JoinLobby(); });
            browseLobbiesButton.onClick.AddListener(() => SceneManager.LoadScene("JoinAvailableGames"));
        }

        private void Start() {
            lobbyIdInputField.Select();
            lobbyIdInputField.ActivateInputField();
        }

        private void JoinLobby() {
            _lobbyID = lobbyIdInputField.text;
            GameManager.Instance.SetLobbyId(_lobbyID);
        
            if (_lobbyID.Equals("")) {
                popUpIdLobbyError.SetActive(true);
                GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("Insert a lobby ID");
            } else { 
                ClientManager.Instance.JoinLobbyAsClient(_lobbyID);
                popUpIdLobbyError.SetActive(true);
                GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("Trying to join the lobby");
                StartCoroutine(AttemptJoinLobby());
            }
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
                .SetErrorText("Unable to join the lobby.\nTry again");
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Return))
                JoinLobby();
        }
    }
}