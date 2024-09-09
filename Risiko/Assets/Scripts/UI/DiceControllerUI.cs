using System.Collections;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class DiceControllerUI : MonoBehaviour {
        [SerializeField] private Button diceButton;
        [SerializeField] private Animator diceAnimator; // Riferimento all'Animator
        [SerializeField] private TMP_Text diceResults;
        private bool _pressedButton = false;

        private void Awake() {
            diceButton.onClick.AddListener(() => {
                // Inizio animazione dei dadi
                diceAnimator.SetBool("Roll", true);
                ThrowDice();
            });
        }

        private void Update() {
            if (GameManager.Instance.GetExtractedNumber() != 0 && _pressedButton) {
                diceAnimator.gameObject.SetActive(false);
                diceAnimator.SetBool("Roll", false);
                string gameOrder = "1 - ";
                for (int i = 1, n = 2; i < GameManager.Instance.getGame_order().Length - 1; i++) {
                    if(GameManager.Instance.getGame_order()[i] == ' ') 
                        gameOrder += " " + n++ + " - ";
                    else 
                        gameOrder += GameManager.Instance.getGame_order()[i];
                }
                diceResults.text = "Extracted number: " + GameManager.Instance.GetExtractedNumber() + "\n Game order: " 
                                   + gameOrder;
                diceResults.gameObject.SetActive(true);
                StartCoroutine(WaitAndLoadScene(1f)); //Delay di qualcosa o un tasto per passare di scena ////ERA 8
                // GameManager.LoadScene("Main") -> scena della mappa
            }
        }

        public void ThrowDice() {
            diceButton.interactable = false;
            StartCoroutine(Wait(4f));
        }

        IEnumerator Wait(float delay) {
            yield return new WaitForSeconds(delay);
            _pressedButton = true;
        }

        IEnumerator WaitAndLoadScene(float delay) {
            // Aspetta il numero di secondi specificato
            yield return new WaitForSeconds(delay);

            // Cambia scena
            SceneManager.LoadScene("Main");
        }
    }
}