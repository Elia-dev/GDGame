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
        [SerializeField] private Animator diceAnimator;
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
                
                string gameOrder = GameManager.Instance.getGame_order();
                gameOrder = gameOrder.Substring(0, gameOrder.Length - 2);
                
                diceResults.text = "Extracted number: " + GameManager.Instance.GetExtractedNumber() + "\n Game order: " 
                                   + gameOrder;
                diceResults.gameObject.SetActive(true);
                StartCoroutine(WaitAndLoadScene(5f));
            }
        }

        private void ThrowDice() {
            diceButton.interactable = false;
            StartCoroutine(Wait(4f));
        }

        private IEnumerator Wait(float delay) {
            yield return new WaitForSeconds(delay);
            _pressedButton = true;
        }

        private IEnumerator WaitAndLoadScene(float delay) {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene("Main");
        }
    }
}