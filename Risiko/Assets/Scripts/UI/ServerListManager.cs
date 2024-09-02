using System.Collections;
using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ServerListManager : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private GameObject popupError;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_InputField serverIPInput;
        [SerializeField] private Button selectButton;
        public GameObject rowPrefab; // Il prefab per la riga
        public Transform contentParent; // Il contenitore (Content) delle righe
        private List<string> _servers = new List<string>();


        private void Awake()
        {
            backButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenu"); });
            refreshButton.onClick.AddListener(async () =>
            {
                await ClientManager.Instance.FetchOnlineServers();
                _servers = ClientManager.Instance.GetOnlineServersList();
                RefreshServerList();
            });
            
            selectButton.onClick.AddListener(() =>
            {
                if (!serverIPInput.text.Equals(""))
                {
                    SetActiveServer(serverIPInput.text);
                }
                else
                {
                    popupError.SetActive(true);
                    GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                        .SetErrorText("Please insert a server IP");
                }
            });
        }


        void Start()
        {
            _servers = ClientManager.Instance.GetOnlineServersList();
            // Per ogni partita disponibile, crea una riga nella lista
            RefreshServerList();
        }

        private void RefreshServerList()
        {
            rowPrefab.SetActive(true);
            foreach (Transform child in contentParent)
            {
                Debug.Log("SCASSO TUTTO ZIO PERA");
                Debug.Log(child.gameObject);
                Destroy(child.gameObject);
            }

            _servers.ForEach(server =>
            {
                GameObject newRow = Instantiate(rowPrefab, contentParent);
                newRow.transform.SetParent(contentParent);

                newRow.transform.Find("serverIP").GetComponent<TMP_Text>().text = "Server: " + server;

                // Aggiungi un listener al click del bottone per restituire l'idLobby
                newRow.GetComponent<Button>().onClick.AddListener(() => FillInputField(server));
            });
            rowPrefab.SetActive(false);
        }
        
        private void FillInputField(string server)
        {
            serverIPInput.text = server;
        }
        
        private void SetActiveServer(string server)
        {
            ClientManager.Instance.SetActiveServer(server);
            popupError.SetActive(true);
            GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                .SetErrorText("Server selected");
        }
    }
}