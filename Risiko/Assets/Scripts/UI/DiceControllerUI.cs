using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DiceControllerUI : MonoBehaviour
{
    
    [SerializeField] private Button DiceButton;
    [SerializeField] private Animator diceAnimator; // Riferimento all'Animator
    [SerializeField] private TMP_Text DiceResults;
    private bool pressedButton = false;
    GameManager gm = GameManager.Instance;
    private void Awake()
    {
        DiceButton.onClick.AddListener(() =>
        {
            diceAnimator.SetBool("Roll", true);
        });
    }

    private void Update()
    {
        if (gm.GetExtractedNumber() != 0 && pressedButton)
        {
            diceAnimator.gameObject.SetActive(false);
            diceAnimator.SetBool("Roll", false);
            DiceResults.text = "Extracted number: " + gm.GetExtractedNumber() + "\n Game order: "+ gm.getGame_order();
            DiceResults.gameObject.SetActive(true);
            StartCoroutine(WaitAndLoadScene(8f)); //Delay di qualcosa o un tasto per passare di scena
            // GameManager.LoadScene("Main") -> scena della mappa
        }
            
    }

    public void TrowDice() {
        StartCoroutine(Wait(4f));
        pressedButton = true;
    }

    IEnumerator Wait(float delay) {
        yield return new WaitForSeconds(delay);
    }
    
    IEnumerator WaitAndLoadScene(float delay)
    {
        // Aspetta il numero di secondi specificato
        yield return new WaitForSeconds(delay);

        // Cambia scena
        SceneManager.LoadScene("Main");
    }
}