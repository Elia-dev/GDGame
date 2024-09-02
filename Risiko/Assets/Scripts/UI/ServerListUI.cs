using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ServerListUI : MonoBehaviour {
        [SerializeField] private Button backButton;
    
        private void Awake() 
        {
            backButton.onClick.AddListener(() => {
                Debug.Log("Exiting server list...");
                SceneManager.LoadScene("MainMenu");
            });

        
        }
    }
}