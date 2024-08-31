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
        public GameObject rowPrefab; // Il prefab per la riga
        public Transform contentParent; // Il contenitore (Content) delle righe
        private static List<Lobby> _lobbies = new List<Lobby>();
        private float _delay = 6.0f; // Durata del ritardo in secondi
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
        
            // Per ogni partita disponibile, crea una riga nella lista
            _lobbies.ForEach(lobby => {
                GameObject newRow = Instantiate(rowPrefab, contentParent);
                newRow.transform.SetParent(contentParent);

                newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = "Lobby: " + lobby.getLobbyID();
                newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = "Host: " + lobby.getHostName();
                newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = "Players: " +lobby.getPlayersNum();

                // Aggiungi un listener al click del bottone per restituire l'idLobby
                newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.getLobbyID()));
            });
            rowPrefab.SetActive(false);
        }

        private void Update() {
            if (_timer > 0) {
                _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
            }

            if (_timer > 3 && !_reloadLobbies) {
                _reloadLobbies = true;
                rowPrefab.SetActive(true);
                foreach (Transform child in contentParent) {
                    Destroy(child.gameObject);
                }

                _lobbies.ForEach(lobby => {
                    /*Debug.Log("Lobby: " + lobby.getLobbyID());
                Debug.Log("host: " + lobby.getHostName());
                Debug.Log("players: " + lobby.getPlayersNum());*/
                    GameObject newRow = Instantiate(rowPrefab, contentParent);
                    //newRow.transform.SetParent(contentParent, false);
                    newRow.transform.SetParent(contentParent);

                    newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = "Lobby: " + lobby.getLobbyID();
                    newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = "Host: " + lobby.getHostName();
                    newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = "Players: " +lobby.getPlayersNum();

                    // Aggiungi un listener al click del bottone per restituire l'idLobby
                    newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.getLobbyID()));
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
                timerConnection -= Time.deltaTime; //DA COPIARE DI LA
                if (ClientManager.Instance.IsConnectedToLobby()) {
                    SceneManager.LoadScene("WaitingRoomClient");
                    yield break; // Esci dalla coroutine se ci si connette alla lobby
                }

                yield return null; // Aspetta il prossimo frame e continua la coroutine
            }

            // Se il timer scade e non ci si è connessi alla lobby, mostra un errore
            GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                .SetErrorText("Unable to join the lobby.\nTry another one.");
        }
    }
}