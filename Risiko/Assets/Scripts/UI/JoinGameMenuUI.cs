using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinGameMenuUI : MonoBehaviour
{
    
    [SerializeField] private Button BackButton;
    [SerializeField] private Button JoinLobbyButton; 
    [SerializeField] TMP_InputField lobbyIdInputField;
    [SerializeField] private GameObject PopUpIdLobbyError;
    private string lobby_id;
    
    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("GameMenu");
        });
		
        JoinLobbyButton.onClick.AddListener(() =>
        {
            JoinLobby();
		});
    }

    private void JoinLobby() {
        lobby_id = lobbyIdInputField.text;
        Player.Instance.LobbyId = lobby_id;
        GameManager.Instance.SetLobbyId(lobby_id);
        Debug.Log("Lobby ID: " + lobby_id);
        if (lobby_id.Equals(""))
        {
            PopUpIdLobbyError.SetActive(true);
        }
        else
        {
            ClientManager.Instance.JoinLobbyAsClient(lobby_id);
            
            SceneManager.LoadScene("WaitingRoomClient");
        }
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
            JoinLobby();
    }
}
