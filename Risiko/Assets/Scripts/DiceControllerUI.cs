using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DiceControllerUI : MonoBehaviour
{
    
    [SerializeField] private Button DiceButton;
    [SerializeField] private Animator diceAnimator; // Riferimento all'Animator
    [SerializeField] private TMP_Text DiceResults;
        

    private void Awake()
    {
        DiceButton.onClick.AddListener(() =>
        {
            diceAnimator.SetBool("Roll", true);
            /* I dadi si muovono finchè tutti i giocatori non hanno cliccato sul tasto? una volta che tutti
             l'hanno fatto disattiviamo i dadi e mettiamo visibile il testo con i risultati dell'estrazione
            
            diceAnimator.gameObject.SetActive(false);
            DiceResults.text = "Qui la lista dei giocatori con i risultati";
            DiceResults.gameObject.SetActive(true);*/
        });
    }
}