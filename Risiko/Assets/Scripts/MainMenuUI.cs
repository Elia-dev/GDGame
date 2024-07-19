using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button ExitButton;

    private ClientManager cm;

    void Start()
    {
        

    }
    
    private void Awake() {
        StartButton.onClick.AddListener(() => {
            cm = ClientManager.Instance;
            Console.WriteLine("Trying to connect to database...");
            cm.StartClient();
            if (cm.IsConnected())
            {
                Console.WriteLine("Connected");
            }
            else
            {
                Console.WriteLine("Connection failed");
            }
            SceneManager.LoadScene("GameMenu");
        });
        
        ExitButton.onClick.AddListener(() => {
            SceneManager.LoadScene("ExitMenu");
        });
    }
}
