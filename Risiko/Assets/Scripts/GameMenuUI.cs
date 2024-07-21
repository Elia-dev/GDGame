using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
	[SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button JoinLobbyButton; 
    [SerializeField] TMP_InputField usernameInputField;
    
    
    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
		
		CreateLobbyButton.onClick.AddListener(() =>
        {
            string username = usernameInputField.text; 
            if (Utils.CheckUsername(username))
            {
                //Debug.Log("Username OK, changing scene from GameMenu to HostMenu");
                Player player = Player.Instance;
                player.Name = username;
                SceneManager.LoadScene("HostMenu");
                //Create Lobby
            }
            else
            {
                //Debug.Log("Invalid username");
                //Popup
            }
		});

        JoinLobbyButton.onClick.AddListener(() =>
        {
            string username = usernameInputField.text;
            if (Utils.CheckUsername(username))
            {
                //Debug.Log("Username OK, changing scene from GameMenu to JoinLobbyMenu");
                Player player = Player.Instance;
                player.Name = username;
                SceneManager.LoadScene("JoinGameMenu");
            }
            else
            {
                //Debug.Log("Invalid username");
                //Popup
            }
        });
    }
}
