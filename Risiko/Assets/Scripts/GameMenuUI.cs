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
            if (username != null)
            {
                Debug.Log("Missing username");
                //Popup
            }
            else if (Utils.CheckUsername(username))
            {
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
                SceneManager.LoadScene("Main");
                //Join Lobby
            }
            else
            {
                Debug.Log("Invalid username");
                //Popup
            }
        });
    }
}
