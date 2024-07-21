using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoingameMenu : MonoBehaviour
{
    
    ClientManager_old cm = ClientManager_old.Instance;
    Player player = Player.Instance;
    
    [SerializeField] private Button BackButton;
    [SerializeField] private Button JoinLobbyButton; 
    [SerializeField] TMP_InputField lobbyIdInputField;
    [SerializeField] private GameObject PopUpIdLobbyError;
    private string lobby_id;
    
    void Start()
    {
        //cm.StartClient();
        //cm.Send(player.Name);
    }
    
    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("GameMenu");
        });
		
        JoinLobbyButton.onClick.AddListener(() =>
        {
            lobby_id = lobbyIdInputField.text;
            player.LobbyId = lobby_id;
            Debug.Log(lobby_id);
            if (lobby_id.Equals(""))
            {
                PopUpIdLobbyError.SetActive(true);
            }
            else
            {
                cm.JoinLobbyAsClient(lobby_id);
                // Attendere in qualche modo che si colleghi alla lobby
                // Appena connesso mostrare la lobby di attesa con gli altri player
            }
		});
    }
}
