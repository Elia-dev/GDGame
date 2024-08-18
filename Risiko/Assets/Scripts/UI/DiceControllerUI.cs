using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

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
            StartCoroutine(WaitAndLoadScene(8f)); //Delay di qualcosa o un tasto per passare di scena
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