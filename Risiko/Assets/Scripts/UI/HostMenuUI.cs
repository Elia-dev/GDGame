using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostMenuUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private Button UpdateButton;
    [SerializeField] private Button RunGameButton;
    [SerializeField] private TMP_Text PlayerList;
    [SerializeField] private TMP_Text LobbyID;
    [SerializeField] private GameObject PopUpDiceHostMenu;
    ClientManager cm = ClientManager.Instance;
    GameManager gm = GameManager.Instance;
    
    private float delay = 1.0f; // Durata del ritardo in secondi
    private float timer;
    private string stringa;
    
    void Start()
    {
        stringa = null;
        cm.CreateLobbyAsHost();
        timer = delay;
    }
    
    private void Update()
    {
        //Debug.Log("LOBBY ID LETTA: " + cm.getLobbyId());
        LobbyID.text = cm.GetLobbyId();
        
        if (timer > 0)
        {
            timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        else
        { 
            cm.SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
            cm.RequestNameUpdatePlayerList();
            
            // Reset del timer
            timer = delay;
            Debug.Log("HOSTMENU - playerList:" + PlayerList.text);
        }
        
        //Aggiornamento lista giocatori
        stringa = string.Join(", ", gm.PlayersName);
        PlayerList.text = "Players: " + stringa;
       

        
        //Quando i giocatori saranno 3+
        if (gm.GetPlayersNumber() >= 3)
        {
            RunGameButton.interactable = true;
        }
        else
        {
            RunGameButton.interactable = false;
        }
        
    }
    private void Awake()
    {
        BackButton.onClick.AddListener(() =>
        {
            //Tell the server that lobby and player are dead
            ClientManager.Instance.KillLobby();
            //resetPlayer
            Player.Instance.resetPlayer();
            //resetGameManager(lobby)
            GameManager.Instance.resetGameManager();
            
            
            SceneManager.LoadScene("GameMenu");
        });
        
        RunGameButton.onClick.AddListener(() =>
        {
            
            cm.StartHostGame();
            PopUpDiceHostMenu.SetActive(true);
        });
        
        UpdateButton.onClick.AddListener(() =>
        {
            cm.RequestNameUpdatePlayerList();
        });
    }
}
