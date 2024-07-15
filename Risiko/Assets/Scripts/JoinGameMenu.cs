using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoingameMenu : MonoBehaviour
{
    
    ClientManager cm = ClientManager.Instance;
    Player player = Player.Instance;
    
    [SerializeField] private Button BackButton;
    [SerializeField] private Button JoinLobbyButton; 
    [SerializeField] InputField lobbyIdInputField;
    private string lobby_id;
    
    void Start()
    {
        cm.StartClient();
        cm.Send(player.Name);
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
            if (lobby_id != null)
            {
                Debug.Log("Missing lobby id");
                // Implementare Popup
            }
            else
            {
                cm.JoinLobbyAsClient(lobby_id);
                // Attendere in qualche modo che si colleghi alla lobby
                // Appena connesso mostrare lalobby di attesa con gli altri player
            }
		});
    }
}
