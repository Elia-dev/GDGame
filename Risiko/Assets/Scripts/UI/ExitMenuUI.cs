using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ExitMenuUI : MonoBehaviour
    {
        [SerializeField] private Button StayButton;
        [SerializeField] private Button QuitButton;

        private void Awake() {
            StayButton.onClick.AddListener(() => {
                SceneManager.LoadScene("MainMenu");
            });
        
            QuitButton.onClick.AddListener(Application.Quit);
        }

    }
}
