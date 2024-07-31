using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DiceControllerUI : MonoBehaviour
{
    
    [SerializeField] private Button DiceButton;
    [SerializeField] private Animator diceAnimator; // Riferimento all'Animator
    [SerializeField] private TMP_Text DiceResults;
    ClientManager cm = ClientManager.Instance;

    private void Awake()
    {
        DiceButton.onClick.AddListener(() =>
        {
            diceAnimator.SetBool("Roll", true);
        });
    }

    private void Update()
    {
        if (cm.GetExtractedNumber() != 0)
        {
            diceAnimator.gameObject.SetActive(false);
            DiceResults.text = cm.GetExtractedNumber() + cm.getGame_order() + cm.GetGameOrderExtractedNumbers();
            DiceResults.gameObject.SetActive(true);
        }
            
    }
}