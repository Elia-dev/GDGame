using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button ExitButton;
    
    ClientManager cm = ClientManager.Instance;

    void start()
    {
        cm.StartClient();
    }
    
    private void Awake() {
        StartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("GameMenu");
        });
        
        ExitButton.onClick.AddListener(() => {
            SceneManager.LoadScene("ExitMenu");
        });
    }
}
