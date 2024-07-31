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

    private Player _player = Player.Instance;
    private ClientManager cm = ClientManager.Instance;
    
    private float delay = 5.0f; // Durata del ritardo in secondi
    private float timer;

    void Start()
    {
        LobbyID.text = _player.LobbyId;
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
            cm.RequestNameUpdatePlayerList();
            cm.SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
            // Reset del timer
            timer = delay;
        }
        
        string stringa = string.Join(" ", cm.NamePlayersTemporaneo);
        PlayerList.text = stringa;
        Debug.Log("WAITING_ROOM - playerList:" + stringa);
        
        
        //Quando l'HOST avvia il gioco
        //PopUpDice.SetActive(true);
    }

    private void Awake()
    {
        BackButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene("GameMenu");
        });
    }
}
