using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomClientUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private TMP_Text PlayerList;
    [SerializeField] private TMP_Text LobbyID;
    [SerializeField] private GameObject PopUpDice;
    
    
    private float delay = 1.0f; // Durata del ritardo in secondi
    private float timer;

    void Start()
    {
        LobbyID.text = GameManager.Instance.GetLobbyId();
        timer = delay;
    }
    
    void Update()
    {
        
        if (timer > 0)
        {
            timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        else
        { 
            ClientManager.Instance.RequestNameUpdatePlayerList();
            ClientManager.Instance.SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
            // Reset del timer
            timer = delay;
            Debug.Log("WAITING_ROOM - playerList:" + PlayerList.text);
        }
        
        string stringa = string.Join(", ", GameManager.Instance.PlayersName);
        PlayerList.text = "Players: " + stringa;
        
        //Quando l'HOST avvia il gioco
        if (!GameManager.Instance.GetGameWaitingToStart())
        {
            Debug.Log("L'HOST HA FATTO COMINCIARE LA PARTITA");
            PopUpDice.SetActive(true);
        }

    }

    private void Awake()
    {
        BackButton.onClick.AddListener(() =>
        {
            ClientManager.Instance.LeaveLobby();
            GameManager.Instance.ResetGameManager();
            SceneManager.LoadScene("GameMenu");
        });
    }
}
