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
        if (gm.GetExtractedNumber() != 0)
        {
            diceAnimator.gameObject.SetActive(false);
            diceAnimator.SetBool("Roll", false);
            DiceResults.text = "Extracted number: " + gm.GetExtractedNumber() + "\n Game order: "+ gm.getGame_order();
            DiceResults.gameObject.SetActive(true);
            StartCoroutine(WaitAndLoadScene(8f)); //Delay di qualcosa o un tasto per passare di scena
            // GameManager.LoadScene("Main") -> scena della mappa
        }
            
    }
    
    IEnumerator WaitAndLoadScene(float delay)
    {
        // Aspetta il numero di secondi specificato
        yield return new WaitForSeconds(delay);

        // Cambia scena
        SceneManager.LoadScene("Main");
    }
}