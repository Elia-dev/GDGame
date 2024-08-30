using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
	[SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton; 
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] private GameObject popupError;

    private void Awake() {
        backButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
            ClientManager.Instance.ResetConnection();
            Debug.Log("Back to Main Menu, connection reset");
        });
		
		createLobbyButton.onClick.AddListener(() =>
        {
            string username = usernameInputField.text; 
            if (Utils.CheckNickname(username).Equals("OK"))
            {
                Debug.Log("Username OK, changing scene from GameMenu to HostMenu");
                Player.Instance.Initialize();
                Player.Instance.Name = username;
                
                SceneManager.LoadScene("HostMenu");
                
            }
            else
            {
                Debug.Log("Invalid username");
                popupError.SetActive(true);
                GameObject.Find("PopUpContainer").GetComponent<PopUpDisplayMessageUI>()
                    .SetErrorText(Utils.CheckNickname(username));
            }
		});

        joinLobbyButton.onClick.AddListener(() =>
        {
            string username = usernameInputField.text;
            if (Utils.CheckNickname(username).Equals("OK"))
            {
                //Debug.Log("Username OK, changing scene from GameMenu to JoinLobbyMenu");
                Player.Instance.Initialize();
                Player.Instance.Name = username;
                SceneManager.LoadScene("JoinGameMenu");
            }
            else
            {
                popupError.SetActive(true);
                GameObject.Find("PopUpContainer").GetComponent<PopUpDisplayMessageUI>()
                    .SetErrorText(Utils.CheckNickname(username));
            }
        });
    }
}
