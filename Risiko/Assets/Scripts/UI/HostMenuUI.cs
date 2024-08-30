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
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HostMenuUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button runGameButton;
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private TMP_Text lobbyID;
    [SerializeField] private GameObject popUpDiceHostMenu;
    [SerializeField] private Button addBotButton;
    [SerializeField] private Button removeBotButton;
    
    private float delay = 1.0f; // Durata del ritardo in secondi
    private float timer;
    private string stringa;
    
    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            ClientManager.Instance.KillLobby();
            Player.Instance.ResetPlayer();
            GameManager.Instance.ResetGameManager();
            
            SceneManager.LoadScene("GameMenu");
        });
        
        runGameButton.onClick.AddListener(() =>
        {
            
            ClientManager.Instance.StartHostGame();
            popUpDiceHostMenu.SetActive(true);
        });
        
        updateButton.onClick.AddListener(() =>
        {
            ClientManager.Instance.RequestNameUpdatePlayerList();
        });
        
        //addBotButton.onClick.AddListener();
        //removeBotButton.onClick.AddListener();
    }
    void Start()
    {
        stringa = null;
        ClientManager.Instance.CreateLobbyAsHost();
        timer = delay;
    }
    
    private void Update()
    {
        //Debug.Log("LOBBY ID LETTA: " + cm.getLobbyId());
        lobbyID.text = GameManager.Instance.GetLobbyId();
        
        if (timer > 0)
        {
            timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        else
        { 
            ClientManager.Instance.SendName(); // Da vedere, se si potesse fare soltanto la prima volta sarebbe meglio
            ClientManager.Instance.RequestNameUpdatePlayerList();
            
            // Reset del timer
            timer = delay;
            Debug.Log("HOSTMENU - playerList:" + playerList.text);
        }
        
        //Aggiornamento lista giocatori
        stringa = string.Join(", ", GameManager.Instance.PlayersName);
        playerList.text = "Players: " + stringa;
       

        
        //Quando i giocatori saranno 3+
        if (GameManager.Instance.GetPlayersNumber() >= 2)
        {
            runGameButton.interactable = true;
        }
        else
        {
            runGameButton.interactable = false;
        }
        
    }
    
}
