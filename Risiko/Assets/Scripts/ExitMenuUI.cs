using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenuUI : MonoBehaviour
{
    [SerializeField] private Button StaytButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private InputField UserInput;
    private string username;

    private void Awake() {
        StaytButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
        
        QuitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }

}
