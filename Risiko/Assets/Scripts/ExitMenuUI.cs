using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenuUI : MonoBehaviour
{
    [SerializeField] private Button StayButton;
    [SerializeField] private Button QuitButton;
    private string username;

    private void Awake() {
        StayButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
        
        QuitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }

}
