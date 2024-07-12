using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
	[SerializeField] private Button CreateLobbyButton;
    [SerializeField] InputField usernameInputField;
    private string username;

    private void Awake() {
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
		
		CreateLobbyButton.onClick.AddListener(() => {
            if (username != null)
            {
                SceneManager.LoadScene("Main");
            } else
            {

            }
		});

        usernameInputField.onValueChanged.AddListener(OnUsernameFieldChange);
    }
    private void OnUsernameFieldChange(string username)
    {
        this.username = username;
    }
}
