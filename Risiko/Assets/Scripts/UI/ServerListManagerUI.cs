using System.Collections.Generic;
using System.Threading.Tasks;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ServerListManagerUI : MonoBehaviour
    {
        [SerializeField] public GameObject rowPrefab;
        [SerializeField] public Transform contentParent;
        [SerializeField] private Button backButton;
        [SerializeField] private GameObject popupMessages;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_InputField serverIPInput;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button x;
        [SerializeField] private GameObject loading;

        private List<string> _servers = new List<string>();

        private void Awake()
        {
            backButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenu"); });
            refreshButton.onClick.AddListener(async () =>
            {
                RetrieveServers();
                serverIPInput.text = "";
            });
            
            selectButton.onClick.AddListener(() =>
            {
                if (!serverIPInput.text.Equals(""))
                {
                    SetActiveServer(serverIPInput.text);
                }
                else
                {
                    popupMessages.SetActive(true);
                    GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                        .SetErrorText("Please insert a server IP");
                }
            });
            
            x.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
                popupMessages.SetActive(false);
            });
        }

        private async Task RetrieveServers()
        {
            refreshButton.interactable = false;
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            loading.SetActive(true);
            await ClientManager.Instance.FetchOnlineServers();
            _servers = ClientManager.Instance.GetOnlineServersList();
            RefreshServerList();
        }

        void Start()
        {
            RetrieveServers();
        }

        private void RefreshServerList()
        {
            rowPrefab.SetActive(true);
            
            _servers.ForEach(server =>
            {
                GameObject newRow = Instantiate(rowPrefab, contentParent);
                newRow.transform.SetParent(contentParent);
                newRow.transform.Find("serverIP").GetComponent<TMP_Text>().text = "Server: " + server;

                newRow.GetComponent<Button>().onClick.AddListener(() => {
                    //FillInputField(server);
                    SetActiveServer(server);
                });
            });
            rowPrefab.SetActive(false);
            loading.SetActive(false);
            refreshButton.interactable = true;
        }

        private void SetActiveServer(string server)
        {
            ClientManager.Instance.SetActiveServer(server);
            popupMessages.SetActive(true);
            GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                .SetErrorText("Server selected");
        }
    }
}
