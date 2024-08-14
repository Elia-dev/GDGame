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
    [SerializeField] TMP_InputField UsernameInputField;
    [SerializeField] private GameObject PopupError;
    
    
    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
		
		CreateLobbyButton.onClick.AddListener(() =>
        {
            string username = UsernameInputField.text; 
            if (Utils.CheckNickname(username))
            {
                //Debug.Log("Username OK, changing scene from GameMenu to HostMenu");
                Player player = Player.Instance;
                player.Initialize();
                player.Name = username;
                SceneManager.LoadScene("HostMenu");
                //Create Lobby
            }
            else
            {
                //Debug.Log("Invalid username");
                PopupError.SetActive(true);
            }
		});

        JoinLobbyButton.onClick.AddListener(() =>
        {
            string username = UsernameInputField.text;
            if (Utils.CheckNickname(username))
            {
                //Debug.Log("Username OK, changing scene from GameMenu to JoinLobbyMenu");
                Player player = Player.Instance;
                player.Initialize();
                player.Name = username;
                SceneManager.LoadScene("JoinGameMenu");
            }
            else
            {
                //Debug.Log("Invalid username");
                PopupError.SetActive(true);
            }
        });
    }
}
