using System;
using System.Collections;
using System.Collections.Generic;
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
        cm.CreateLobbyAsHost(); 
        
        //Visualizzazione copdice lobby
        //LobbyID.text = "1518";

    }
    
    private void Update()
    {
        //Debug.Log("LOBBY ID LETTA: " + cm.getLobbyId());
        LobbyID.text = cm.getLobbyId(); //Fare lo show di questo a manetta,
        
        //prima o poi ci sarà qualcosa di settato in quanto il server prima o poi risponderà
        
        // Appena viene premuto il bottone start cambiare scena per far cominciare la partita
        
        //request update
        
        //Aggiornamento lista giocatori
        PlayerList.text = cm.name_players_temporaneo;

        //Quando i giocatori saranno 3+
        //RunGameButton.interactable = true;
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
