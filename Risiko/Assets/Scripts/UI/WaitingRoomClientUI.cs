using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WaitingRoomClientUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private TMP_Text lobbyID;
    [SerializeField] private GameObject popUpDice;
    private float _delay = 1.0f; // Durata del ritardo in secondi
    private float _timer;

    void Start()
    {
        lobbyID.text = GameManager.Instance.GetLobbyId();
        _timer = _delay;
    }
    
    void Update()
    {
        
        if (_timer > 0)
        {
            _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        else
        { 
            ClientManager.Instance.RequestNameUpdatePlayerList();
            ClientManager.Instance.SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
            // Reset del timer
            _timer = _delay;
            Debug.Log("WAITING_ROOM - playerList:" + playerList.text);
        }
        
        string stringa = string.Join(", ", GameManager.Instance.PlayersName);
        playerList.text = "Players: " + stringa;
        
        //Quando l'HOST avvia il gioco
        if (!GameManager.Instance.GetGameWaitingToStart())
        {
            Debug.Log("L'HOST HA FATTO COMINCIARE LA PARTITA");
            //popUpDice.SetActive(true);
            //Invio messaggio per skip
            //SceneManager.LoadScene("Main");
        }

    }

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            ClientManager.Instance.LeaveLobby();
            GameManager.Instance.ResetGameManager();
            SceneManager.LoadScene("JoinGameMenu");
        });
    }
}
