using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
	[SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button JoinLobbyButton; 
    [SerializeField] InputField usernameInputField;
    private string username;
    
    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
		
		CreateLobbyButton.onClick.AddListener(() =>
        {
            username = usernameInputField.text;
            Debug.Log(username);
            if (username != null)
            {
                Debug.Log("Missing username");
                // Implementare Popup
            }
            else if (Utils.CheckUsername(username))
            {
                Debug.Log("Username OK, changing scene from GameMenu to HostMenu");
                Player player = Player.Instance;
                player.Name = username;
                SceneManager.LoadScene("HostMenu");
                //Create Lobby
            }
            else
            {
                Debug.Log("Invalid username");
                //Popup
            }
		});

        JoinLobbyButton.onClick.AddListener(() =>
        {
            username = usernameInputField.text;
            if (username != null)
            {
                Debug.Log("Missing username");
                //Popup
            }
            else if (Utils.CheckUsername(username))
            {
                Debug.Log("Username OK, changing scene from GameMenu to JoinLobbyMenu");
                Player player = Player.Instance;
                player.Name = username;
                SceneManager.LoadScene("JoinGameMenu");
            }
            else
            {
                Debug.Log("Invalid username");
                //Popup
            }
        });
    }
}
