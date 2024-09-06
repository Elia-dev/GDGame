using System.Threading.Tasks;
using Audio;
using businesslogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour {
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button serverListButton;
        [SerializeField] private GameObject popUpConnection;
        [SerializeField] private Button x;
    
        private bool _pressedButton = false;
        private readonly float _delay = 12.0f; // Durata del ritardo in secondi
        private float _timer;

        private void Awake() {
            startButton.onClick.AddListener(() => {
                popUpConnection.SetActive(true);
                x.gameObject.SetActive(false);
                popUpConnection.GetComponentInChildren<DisplayMessageOnPopUpUI>().SetErrorText("Connecting to server...");
                _pressedButton = true;
                _timer = _delay;
                ClientManager.Instance.StartClient();
            });
            optionsButton.onClick.AddListener(() => { SceneManager.LoadScene("OptionsMenu"); });
            exitButton.onClick.AddListener(() => { SceneManager.LoadScene("ExitMenu"); });
            serverListButton.onClick.AddListener(() => { SceneManager.LoadScene("ServerListMenu"); });
        }

        async void Start()
        {
            BGMusic_selector.Instance.SetVolume(PlayerPrefs.GetFloat("musicVolume", 1.0f));
            SFX_selector.Instance.SetVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));
            /*AudioListener.volume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
            AudioListener.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);*/
        }
    
        private void Update() {
            if (_pressedButton) {
                ClientManager.Instance.StartClient();
                if (_timer > 0) {
                    _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
                    if (ClientManager.Instance.IsConnected())
                    {
                        SceneManager.LoadScene("GameMenu");
                    }
                } else {
                    _pressedButton = false;
                    popUpConnection.GetComponentInChildren<DisplayMessageOnPopUpUI>()
                        .SetErrorText("Unable to connect to server.\n Please check your internet connection!");
                    x.gameObject.SetActive(true);
                    _timer = _delay;
                }
            }
        }
    }
}