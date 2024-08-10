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
            DiceResults.text = gm.GetExtractedNumber() + gm.getGame_order() + gm.GetGameOrderExtractedNumbers();
            DiceResults.gameObject.SetActive(true);
            //Delay di qualcosa o un tasto per passare di scena
            // GameManager.LoadScene("Main") -> scena della mappa
        }
            
    }
}