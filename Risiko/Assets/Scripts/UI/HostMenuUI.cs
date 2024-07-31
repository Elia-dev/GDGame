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
    Player player = Player.Instance;
    
    private float delay = 5.0f; // Durata del ritardo in secondi
    private float timer;
    private Player Player = Player.Instance;
    private string stringa;
    
    void Start()
    {
        stringa = null;
        cm.CreateLobbyAsHost();
        /*
        do
        {
            Debug.Log("Waiting for playerID");
        } while (player.PlayerId == null);
        */
        //cm.SendName(); // Mossa pericolosa, avrà già ricevuto l'id dal server quando esegue questo comando?
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
        stringa = string.Join(" ", gm.PlayersName);
        PlayerList.text = stringa;
       

        
        //Quando i giocatori saranno 3+
        if (gm.GetPlayersNumber() > 3)
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
