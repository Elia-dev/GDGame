using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class OptionsMenuUI : MonoBehaviour {
        [SerializeField] private Button backButton;
    
        private void Awake() 
        {
            backButton.onClick.AddListener(() => {
                Debug.Log("Exiting options menu...");
                SceneManager.LoadScene("MainMenu");
            });

        
        }
    }
}