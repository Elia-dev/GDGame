using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class ExitMenuUI : MonoBehaviour
    {
        [SerializeField] private Button stayButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            stayButton.onClick.AddListener(() => {
                SceneManager.LoadScene("MainMenu");
            });
        
            quitButton.onClick.AddListener(Application.Quit);
        }

    }
}
