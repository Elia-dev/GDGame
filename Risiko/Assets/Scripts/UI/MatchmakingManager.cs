using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchmakingManager : MonoBehaviour
{
    public GameObject rowPrefab; // Il prefab per la riga
    public Transform contentParent; // Il contenitore (Content) delle righe
    private static List<Lobby> _lobbies = new List<Lobby>();
    private float _delay = 7.0f; // Durata del ritardo in secondi
    private float _timer;
    void Start() {
        _timer = _delay;
        /*
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }*/
        // Per ogni partita disponibile, crea una riga nella lista
        _lobbies.ForEach(lobby =>
        {
            /*Debug.Log("Lobby: " + lobby.getLobbyID());
            Debug.Log("host: " + lobby.getHostName());
            Debug.Log("players: " + lobby.getPlayersNum());*/
            GameObject newRow = Instantiate(rowPrefab, contentParent);
            //newRow.transform.SetParent(contentParent, false);
            newRow.transform.SetParent(contentParent);

            newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = lobby.getLobbyID();
            newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = lobby.getHostName(); 
            newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = lobby.getPlayersNum().ToString();

            // Aggiungi un listener al click del bottone per restituire l'idLobby
            newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.getLobbyID()));
            
        });
        rowPrefab.SetActive(false);
        /*
        for(int i = 0; i < _lobbies.Count; i++)
        {
            Debug.Log("Lobby: " + _lobbies[i].getLobbyID() + " i = " + i);
            Debug.Log("host: " + _lobbies[i].getHostName()+ " i = " + i);
            Debug.Log("players: " + _lobbies[i].getPlayersNum().ToString()+ " i = " + i);
            GameObject newRow = Instantiate(rowPrefab, contentParent);
            newRow.transform.SetParent(contentParent, false);

            newRow.transform.Find("idLobbyText").GetComponent<TextMeshProUGUI>().text = _lobbies[i].getLobbyID();
            newRow.transform.Find("hostNameText").GetComponent<TextMeshProUGUI>().text = _lobbies[i].getHostName(); 
            newRow.transform.Find("numPlayersText").GetComponent<TextMeshProUGUI>().text = _lobbies[i].getPlayersNum().ToString();

            // Aggiungi un listener al click del bottone per restituire l'idLobby
            newRow.GetComponent<Button>().onClick.AddListener(() => SelectLobby(_lobbies[i].getLobbyID()));
        }
        */
    }

    private void Update() {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
        }
        
        if (_timer > 4) {
            ClientManager.Instance.RequestAllGames();
        }
        
        if(_timer > 6) {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            _lobbies.ForEach(lobby =>
            {
                /*Debug.Log("Lobby: " + lobby.getLobbyID());
                Debug.Log("host: " + lobby.getHostName());
                Debug.Log("players: " + lobby.getPlayersNum());*/
                GameObject newRow = Instantiate(rowPrefab, contentParent);
                //newRow.transform.SetParent(contentParent, false);
                newRow.transform.SetParent(contentParent);

                newRow.transform.Find("idLobbyText").GetComponent<TMP_Text>().text = lobby.getLobbyID();
                newRow.transform.Find("hostNameText").GetComponent<TMP_Text>().text = lobby.getHostName(); 
                newRow.transform.Find("numPlayersText").GetComponent<TMP_Text>().text = lobby.getPlayersNum().ToString();

                // Aggiungi un listener al click del bottone per restituire l'idLobby
                newRow.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.getLobbyID()));
            
            });
        }

        if (_timer <= 0) {
            _timer = _delay;
        }
    }

    public static void LoadAvailableLobbies(List<Lobby> availableLobbies)
    {
        _lobbies = availableLobbies;
    }

    /*private void SelectLobby(string idLobby)
    {
        // Azione da intraprendere quando una partita viene selezionata
        Debug.Log("Hai selezionato la partita con id: " + idLobby);
        // Puoi ora implementare il collegamento alla lobby o altro
    }*/
    
    private void JoinLobby(string idLobby) {
        GameManager.Instance.SetLobbyId(idLobby);
        //Debug.Log("Lobby ID: " + idLobby);
        ClientManager.Instance.JoinLobbyAsClient(idLobby);
        SceneManager.LoadScene("WaitingRoomClient");
        /*if (idLobby.Equals(""))
        {
            popUpIdLobbyError.SetActive(true);
        }
        else
        {
            ClientManager.Instance.JoinLobbyAsClient(idLobby);
            SceneManager.LoadScene("WaitingRoomClient");
        }*/
    }
}
