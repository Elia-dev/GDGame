using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class EndGameUI : MonoBehaviour {
        [SerializeField] private Button exitButton;
        [SerializeField] private TMP_Text endGameResult;
        private void Awake() {
            exitButton.onClick.AddListener(() => {
                GameManagerUI.ThisIsTheEnd = true;
                Player.Instance.ResetPlayer();
                GameManager.Instance.ResetGameManager();
                ClientManager.Instance.ResetConnection();
                //Debug.Log("Resettato tutto dopo fine partita, torno al mainMenu");
                SceneManager.LoadScene("MainMenu");
            });
        }

        public void SetPopUp(string playerId) {
            gameObject.SetActive(true);
            if (playerId.Equals(Player.Instance.PlayerId)) {
                endGameResult.color = Color.green;
                endGameResult.text = "You're the WINNER!";
            }
            else {
                endGameResult.color = Color.red;
                endGameResult.text = "The winner is " + GameManager.Instance.GetEnemyNameById(playerId) + "!";
            }
        }
    }
}
