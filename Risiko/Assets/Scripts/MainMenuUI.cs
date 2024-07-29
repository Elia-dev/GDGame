using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button ExitButton;

    private ClientManager cm;
    
    private void Awake() {
        StartButton.onClick.AddListener( () => {
            cm = ClientManager.Instance;
            Debug.Log("Trying to connect to database...");
            cm.StartClient();
            Debug.Log("CONNECTED");
            SceneManager.LoadScene("GameMenu");
        });
        
        ExitButton.onClick.AddListener(() => {
            SceneManager.LoadScene("ExitMenu");
        });
    }
}
