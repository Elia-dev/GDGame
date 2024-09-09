using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class EscMenuUI : MonoBehaviour {
        [SerializeField] private GameObject volumeMenu;
        [SerializeField] private GameObject exitMenu;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button exitButton;

        private void Awake() {
            backButton.onClick.AddListener(() => this.gameObject.SetActive(false));
            optionsButton.onClick.AddListener(() => {
                this.gameObject.SetActive(false);
                volumeMenu.SetActive(true);
            });
            exitButton.onClick.AddListener(() => {
                this.gameObject.SetActive(false);
                exitMenu.SetActive(true);
            });
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                gameObject.SetActive(false);
            }
        }

        public void BackForVolumeMenu() {
            volumeMenu.SetActive(false);
            this.gameObject.SetActive(true);
        }
    
        public void BackButtonForExitMenu() {
            exitMenu.SetActive(false);
            this.gameObject.SetActive(true);
        }

        public void ExitButtonForExitMenu() {
            if (!GameManager.Instance.getKillerId().Equals("")) 
            {
                // Se sono stato ucciso da qualcuno esco senza far chiudere la partita a tutti
                ClientManager.Instance.LeaveLobby();
                Player.Instance.ResetPlayer();
                GameManager.Instance.ResetGameManager();
                ClientManager.Instance.ResetConnection();
            }
            else
            {
                ClientManager.Instance.LeaveGame();
                Player.Instance.ResetPlayer();
                GameManager.Instance.ResetGameManager();
                ClientManager.Instance.ResetConnection();
            }
            SceneManager.LoadScene("MainMenu");
        }
    }
}
