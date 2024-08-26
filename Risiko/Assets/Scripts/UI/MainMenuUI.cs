using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private GameObject popUpConnection;
    [SerializeField] private Button x;
    
    private bool _pressedButton = false;
    private float _delay = 12.0f; // Durata del ritardo in secondi
    private float _timer;

    private void Awake() {
        startButton.onClick.AddListener(() => {
            popUpConnection.SetActive(true);
            x.gameObject.SetActive(false);
            popUpConnection.GetComponentInChildren<PopUpBadNameUI>().SetErrorText("Connecting to server...");
            Debug.Log("Trying to connect to database...");
            _pressedButton = true;
            _timer = _delay;
            ClientManager.Instance.StartClient();
        });
        optionsButton.onClick.AddListener(() => { SceneManager.LoadScene("OptionsMenu"); });
        exitButton.onClick.AddListener(() => { SceneManager.LoadScene("ExitMenu"); });
    }

    private void Update() {
        if (_pressedButton) {
            ClientManager.Instance.StartClient();
            if (_timer > 0) {
                _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
                if (ClientManager.Instance.IsConnected())
                    SceneManager.LoadScene("GameMenu");
            }
            else {
                _pressedButton = false;
                popUpConnection.GetComponentInChildren<PopUpBadNameUI>()
                    .SetErrorText("Unable to connect to server.\n Please check your internet connection!");
                x.gameObject.SetActive(true);
                _timer = _delay;
            }
        }
    }
}