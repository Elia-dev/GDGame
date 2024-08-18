using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject popUpConnection;
    [SerializeField] private Button x;
    private bool pressedButton = false;
    private float delay = 10.0f; // Durata del ritardo in secondi
    private float timer;
    
    private void Awake() {
        startButton.onClick.AddListener( () => {
            popUpConnection.SetActive(true);
            Debug.Log("Trying to connect to database...");
            pressedButton = true;
            timer = delay;
            ClientManager.Instance.StartClient();
        });
        
        exitButton.onClick.AddListener(() => {
            SceneManager.LoadScene("ExitMenu");
        });
    }

    private void Update() {
        if (pressedButton) {
            ClientManager.Instance.StartClient();
            if (timer > 0)
            {
                timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
                if(ClientManager.Instance.IsConnected())
                    SceneManager.LoadScene("GameMenu");
            }
            else {
                pressedButton = false;
                popUpConnection.GetComponent<PopUpBadNameUI>().SetErrorText("Unable to connect to server.\n " +
                                                                            "Please check your internet connection!");
                x.gameObject.SetActive(true);
                timer = delay;
            }
        }
    }
}
