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
    ClientManager cm = ClientManager.Instance;
    Player player = Player.Instance;
    
    private float delay = 5.0f; // Durata del ritardo in secondi
    private float timer;

    private string stringa;
    // Start is called before the first frame update
    /*
    void Start()
    {
        //cm.Send(player.Name);
        //cm.CreateLobbyAsHost(); // DAL SERVER DEVE PRIMA ESSERE ARRIVATO L'ID DEL PLAYER (da implementare)
        
        //Visualizzazione copdice lobby
        //LobbyID.text = "1518";

    }
*/
    /*
    private void Update()
    {
        //cm.getLobbyId(); //Fare lo show di questo a manetta,
                         //prima o poi ci sarà qualcosa di settato in quanto il server prima o poi risponderà
        
                         // Appena viene premuto il bottone start cambiare scena per far cominciare la partita
                         // DUBBIO: Come aggiorno i giocatori nella lobby di attesa appena si connettono al server?
        
        //Aggiornamento lista giocatori
        //PlayerList.text = "P1 P2 ...";
        
        //Quando i giocatori saranno 3+
        //RunGameButton.interactable = true;
    }
    */
    
    void Start()
    {
        stringa = null;
        cm.CreateLobbyAsHost(); 
        timer = delay;
        //Visualizzazione copdice lobby
        //LobbyID.text = "1518";

    }
    
    private void Update()
    {
        //Debug.Log("LOBBY ID LETTA: " + cm.getLobbyId());
        LobbyID.text = cm.getLobbyId();
        if (timer > 0)
        {
            timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        else
        {
            cm.RequestNameUpdatePlayerList();
            // Reset del timer
            timer = delay;
        }
        
        //Aggiornamento lista giocatori
        stringa = string.Join(" ", cm.NamePlayersTemporaneo);
        PlayerList.text = stringa;
        Debug.Log("HOSTMENU - playerList:" + stringa);

        
        //Quando i giocatori saranno 3+
        if (cm.NamePlayersTemporaneo.Count > 3)
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
        
        UpdateButton.onClick.AddListener(() =>
        {
            cm.RequestNameUpdatePlayerList();
        });
    }
}
