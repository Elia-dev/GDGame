using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class JoinGameMenuUI : MonoBehaviour
{
    
    [SerializeField] private Button backButton;
    [SerializeField] private Button joinLobbyButton; 
    [SerializeField] private TMP_InputField lobbyIdInputField;
    [SerializeField] private GameObject popUpIdLobbyError;
    private string _lobbyID;
    
    private void Awake() {
        backButton.onClick.AddListener(() => {
            GameManager.Instance.ResetGameManager();
            Player.Instance.resetPlayer();
            SceneManager.LoadScene("GameMenu");
        });
		
        joinLobbyButton.onClick.AddListener(() =>
        {
            JoinLobby();
		});
    }

    private void JoinLobby() {
        _lobbyID = lobbyIdInputField.text;
        GameManager.Instance.SetLobbyId(_lobbyID);
        Debug.Log("Lobby ID: " + _lobbyID);
        if (_lobbyID.Equals(""))
        {
            popUpIdLobbyError.SetActive(true);
        }
        else
        {
            ClientManager.Instance.JoinLobbyAsClient(_lobbyID);
            SceneManager.LoadScene("WaitingRoomClient");
        }
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
            JoinLobby();
    }
}
