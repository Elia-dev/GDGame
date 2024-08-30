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
                diceAnimator.SetBool("Roll", true);
                ThrowDice();
            });
        }

        private void Update() {
            if (GameManager.Instance.GetExtractedNumber() != 0 && _pressedButton) {
                diceAnimator.gameObject.SetActive(false);
                diceAnimator.SetBool("Roll", false);
                diceResults.text = "Extracted number: " + GameManager.Instance.GetExtractedNumber() + "\n Game order: " 
                                   + GameManager.Instance.getGame_order();
                diceResults.gameObject.SetActive(true);
                StartCoroutine(WaitAndLoadScene(1f)); //Delay di qualcosa o un tasto per passare di scena ////ERA 8
                // GameManager.LoadScene("Main") -> scena della mappa
            }
        }

        public void ThrowDice() {
            diceButton.interactable = false;
            StartCoroutine(Wait(1f)); ///ERA 4
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