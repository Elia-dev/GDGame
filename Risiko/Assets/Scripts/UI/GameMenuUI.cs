using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class GameMenuUI : MonoBehaviour {
        [SerializeField] private Button backButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] TMP_InputField usernameInputField;
        [SerializeField] private GameObject popupError;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                SceneManager.LoadScene("MainMenu");
                ClientManager.Instance.ResetConnection();
            });

            createLobbyButton.onClick.AddListener(() => {
                string username = usernameInputField.text;
                if (Utils.CheckNickname(username).Equals("OK")) {
                    Player.Instance.Initialize();
                    Player.Instance.Name = username;

                    SceneManager.LoadScene("HostMenu");
                }
                else {
                    popupError.SetActive(true);
                    GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                        .SetErrorText(Utils.CheckNickname(username));
                }
            });

            joinLobbyButton.onClick.AddListener(() => {
                string username = usernameInputField.text;
                if (Utils.CheckNickname(username).Equals("OK")) {
                    Player.Instance.Initialize();
                    Player.Instance.Name = username;
                    SceneManager.LoadScene("JoinGameMenu");
                }
                else {
                    popupError.SetActive(true);
                    GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                        .SetErrorText(Utils.CheckNickname(username));
                }
            });
        }

        private void Start() {
            usernameInputField.Select();
            usernameInputField.ActivateInputField();
        }
    }
}