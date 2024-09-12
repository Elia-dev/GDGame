using System;
using System.Collections;
using System.Linq;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public struct SelectedTerritories {
        public Territory[] territories;
        public int[] count;
    }

    public class TerritoriesManagerDistrPhaseUI : TerritoriesManagerUI {
        [SerializeField] private GameObject popUpAddTank;
        [SerializeField] private TMP_Text stateNameAddTank;
        [SerializeField] private TMP_Text tankNumber;
        [SerializeField] private TMP_Text tankToAdd;
        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private GameObject gameManager;
        [SerializeField] private GameObject escMenu;
        [SerializeField] private GameObject popUpPlayerLeftGame;

        private SelectedTerritories _selectedTerritories;
        private bool _isTurnInitialized = false; // Variabile per tracciare se il turno è stato inizializzato
        private int _armyNumber;
        private static bool _iAmAlive = true;

        public int ArmyNumber => _armyNumber;

        private void Awake() {
            plusButton.onClick.AddListener(() => AddArmy());
            minusButton.onClick.AddListener(() => RemoveArmy());
            endTurnButton.onClick.AddListener(() => {
                if (distributionPhase || TerritoriesManagerGamePhaseUI.ReinforcePhase) {
                    SendArmy();
                } // Se è la fase di attacco sistemo le booleane e invio al server i territori aggiornati
                else if (TerritoriesManagerGamePhaseUI.AttackPhase) {
                    TerritoriesManagerGamePhaseUI.AttackPhase = false;
                    GameManagerUI.AttackPhase = false;
                    if (TerritoriesManagerGamePhaseUI.FirstTurn)
                        TerritoriesManagerGamePhaseUI.FirstTurn = false;
                    ClientManager.Instance.UpdateTerritoriesState();
                    endTurnButton.interactable = false;
                    Debug.Log("IsTurnInitialized = false");
                    //Attendo che Player.Instance.IsMyTurn diventi false
                    StartCoroutine(WaitForTurnToEnd());
                }
            });
        }

        private IEnumerator WaitForTurnToEnd()
        {
            // Attendi finché Player.Instance.IsMyTurn è true
            while (Player.Instance.IsMyTurn)
            {
                yield return null; // Attendi il frame successivo
            }

            // Esegui il codice per il cambio di turno
            OnTurnEnded();
        }

        private void OnTurnEnded()
        {
            // Codice da eseguire quando il turno è terminato
            TerritoriesManagerGamePhaseUI.IsTurnInitialized = false;
        }
        
        public void Start() {
            GameManagerUI.DistributionPhase = true;
        }

        //Scorre la lista dei territori territori assegnati al giocatori per ognuno di questi:
        // Trova il corrispondente territorio sulla mappa ed:
        //  attiva il collider così che sia interagibile
        //  imposta il colore dello stato come quello dell'armata scelta
        public void ActivateTerritories() {
            popUpAddTank.GetComponent<Image>().color =
                Utils.ColorCode(Player.Instance.ArmyColor, 255); //TerritoryHandlerUI.UserColor;
            foreach (var territory in GameManager.Instance.AllTerritories) {
                GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
                if (terr is not null) {
                    terr.GetComponent<PolygonCollider2D>().enabled = true;
                    string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                    terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 50);
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 50);
                }
            }
        }

        public void AddArmy() {
            int totalArmy = _selectedTerritories.count.Sum(); //Numero totale di armate da posizionare in questo turno
            if (totalArmy < _armyNumber) {
                // Se il totale è inferiore a quelle totali posizionabili
                // Cerca la posizione del territorio nel vettore delle armate selezionate nel turno
                int result = FindTerritory(selectedTerritory.name);

                if (result == -1) {
                    //Vuol dire che nessuna armata è stata posizionata su quello stato in questo turno
                    //Cerca il primo posto vuoto nel vettore ed inserisce il territorio corrispondente
                    for (int i = 0; i < _selectedTerritories.count.Length; i++)
                        if (_selectedTerritories.count[i] == 0) {
                            _selectedTerritories.territories[i] = TerritoryInformationsPlayer(selectedTerritory.name);
                            //Incrementa il numero di armate che saranno posizionate sul territorio
                            _selectedTerritories.count[i]++;
                            tankToAdd.text = _selectedTerritories.count[i] + "";
                            i = _selectedTerritories.count.Length;
                        }
                }
                else {
                    //Se il territorio è già nella lista allora è sufficiente incrementare il numero di armate
                    _selectedTerritories.count[result]++;
                    tankToAdd.text = _selectedTerritories.count[result] + "";
                }

                //Seleziona lo stato
                selectedTerritory.Select();
                CheckTotalArmy();
            }
        }

        //Rimuove il numero di armate da posizionare su uno stato (nel caso ne fossero già state assegnate a questo turno)
        public void RemoveArmy() {
            if (int.Parse(tankToAdd.text) > 0) {
                int result = FindTerritory(selectedTerritory.name);

                _selectedTerritories.count[result]--;
                tankToAdd.text = _selectedTerritories.count[result] + "";
                if (_selectedTerritories.count[result] == 0) {
                    _selectedTerritories.territories[result] = null;
                    selectedTerritory.Deselect();
                }

                CheckTotalArmy();
            }
        }

        //Attiva il tasto per il passaggio del turno che permetterà quindi di inviare le armate
        private void CheckTotalArmy() {
            if (_selectedTerritories.count.Sum() == _armyNumber)
                endTurnButton.interactable = true;
            else
                endTurnButton.interactable = false;
        }

        // Aggiorna il numero di armate nei varri territori del Player ed invia al server di aggiornare i territori
        private void SendArmy() {
            for (int i = 0; i < _armyNumber; i++) {
                Player.Instance.Territories.ForEach(terr => {
                    if (_selectedTerritories.territories[i] is not null &&
                        terr.id.Equals(_selectedTerritories.territories[i].id)) {
                        terr.num_tanks += _selectedTerritories.count[i];
                        base.territories.Find(obj => obj.name.Equals(terr.id))
                            .GetComponent<TerritoryHandlerUI>().Deselect();
                    }
                });
            }

            popUpAddTank.SetActive(false); //Toglie il popup dei tank
            Player.Instance.TanksAvailable -= _selectedTerritories.count.Sum(); //Decrementa le armate disponinbili
            Player.Instance.TanksPlaced += _selectedTerritories.count.Sum(); //Incrementa i carri posizionati
            gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
            ClientManager.Instance.UpdateTerritoriesState();
            endTurnButton.interactable = false; //Disattiva il tasto per il passaggio del turno
            _isTurnInitialized = false;

            // Se siamo nella fase di attacco aggiusta le booleane ed aggiorna i territori
            if (!distributionPhase) {
                this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = false;
                TerritoriesManagerGamePhaseUI.ReinforcePhase = false;
                GameManagerUI.ReinforcePhase = false; // Per barra dx
                TerritoriesManagerGamePhaseUI.AttackPhase = true;
                GameManagerUI.AttackPhase = true; // Per barra dx
                this.GetComponent<TerritoriesManagerGamePhaseUI>().IsPhaseGoing = false;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().RefreshTerritories();
                endTurnButton.GetComponentInChildren<TMP_Text>().text = "End Turn!";
                endTurnButton.interactable = true;
            }
        }

        //restituisce l'indice del territorio all'interno del vettore dei territori su cui posizionare le armate
        private int FindTerritory(string territoryName) {
            for (int i = 0; i < _armyNumber; i++) {
                if (_selectedTerritories.territories[i] is not null &&
                    _selectedTerritories.territories[i].id.Equals(territoryName))
                    return i;
            }

            return -1;
        }

        private void Update() {
            if (!GameManager.Instance.GetGameRunning()) {
                popUpPlayerLeftGame.SetActive(true);
                popUpPlayerLeftGame.GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("Player left the game\nyou will be redirected to the main menu...");
                Debug.Log("Game running = false (TerritoriesManagerGamePhaseUI)");
            }

            if (!GameManager.Instance.GetKillerId().Equals("") && _iAmAlive) {
                _iAmAlive = false;
                popUpPlayerLeftGame.SetActive(true);
                popUpPlayerLeftGame.GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("You have been destroyed by "
                                  + GameManager.Instance.GetEnemyNameById(GameManager.Instance.GetKillerId())
                                  + "!\n<i>Now you will be a spectator of a world in which you no longer have influence...</i>");
            }
            if (Player.Instance.IsMyTurn && !_isTurnInitialized) {
                StartTurn();
            }

            if (Input.GetMouseButtonDown(0) && Player.Instance.IsMyTurn) {
                // Controlla se ci sono Canvas attivi così ignora il click
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    // Controlla se il canvas è in modalità Screen Space - Overlay
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        // Controlla se il Canvas è attivo e se ha GameObject attivi
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }

                //Se il collider colpito è un territorio allora lo seleziona altrimenti ignora
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
                RaycastHit2D hit = new RaycastHit2D();
                //Ciclo che evita che non sia possibile selezionare tutto ciò che sta dietro il popup
                foreach (RaycastHit2D hitted in hits) {
                    Collider2D hittedCollider = hitted.collider;
                    if (hittedCollider is BoxCollider2D) {
                        hit = hitted; //new RaycastHit2D();
                        break;
                    }

                    if (hittedCollider is PolygonCollider2D)
                        hit = hitted;
                }

                if (hit.collider is PolygonCollider2D) {
                    TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                    if (territoryHandlerUI is not null) {
                        selectedTerritory = territoryHandlerUI;
                        SelectState(selectedTerritory);
                    }
                }
                else if (hit.collider is null) {
                    gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                    popUpAddTank.SetActive(false);
                    if (selectedTerritory is not null) {
                        selectedTerritory = null;
                    }
                }
            }

            //Se non è il turnoi del giocatore allora mostra SOLO le informazioni del territorio
            if (!Player.Instance.IsMyTurn) {
                if (Input.GetMouseButtonDown(0)) {
                    Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                    foreach (Canvas canvas in allCanvases) {
                        // Controlla se il canvas è in modalità Screen Space - Overlay
                        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                            // Controlla se il Canvas è attivo e se ha GameObject attivi
                            if (canvas.gameObject.activeInHierarchy) {
                                return;
                            }
                        }
                    }

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                    if (hit.collider is not null) {
                        TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                        if (territoryHandlerUI is not null) {
                            gameManager.GetComponent<GameManagerUI>()
                                .ShowTerritoryInfo(TerritoryInformationsAllPlayers(territoryHandlerUI.gameObject.name));
                        }
                    }
                    else {
                        gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                    }
                }
            }

            // Effettua il cambio di scena se si è in game phase
            if (GameManager.Instance.GetGamePhase() && distributionPhase) {
                TerritoriesManagerUI.distributionPhase = false;
                GameManagerUI.DistributionPhase = false;
                this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = false;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = true;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().ActivateOtherPlayersTerritories();
            }

            //Gestion del tasto ESC
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    // Controlla se il canvas è in modalità Screen Space - Overlay
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        // Controlla se il Canvas è attivo e se ha GameObject attivi
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }

                if (popUpAddTank.activeInHierarchy) {
                    popUpAddTank.SetActive(false);
                    if (selectedTerritory is not null) selectedTerritory = null;
                }
                else
                    escMenu.SetActive(true);
            }
        }

        //Metodo che inizializza la struct per la selezione dei territori e attiva il turno
        public void StartTurn() {
            _isTurnInitialized = true; // Attiva il turno
            // Cattura le armate da posizionare
            _armyNumber = Player.Instance.TanksAvailable;
            if (_armyNumber > 3 && distributionPhase) {
                _armyNumber = 3;
            }

            _selectedTerritories.territories = new Territory[_armyNumber];
            _selectedTerritories.count = new int[_armyNumber];
            /*if(!distributionPhase && Player.Instance.TanksAvailable == 0) {
                endTurnButton.interactable = true;
                endTurnButton.onClick.Invoke();
            }*/
        }

        //Trova un territorio nei territori del Player dato l'id del territorio
        private Territory TerritoryInformationsPlayer(string id) {
            return Player.Instance.Territories.Find(x => x.id.Equals(id));
        }

        //Trova un territorio nei territori di tutti i giocatori dato l'id del territorio
        private Territory TerritoryInformationsAllPlayers(string id) {
            return GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        }

        //Mostra il popup per aggiungere o togliere armate
        public void SelectState(TerritoryHandlerUI newTerritory) {
            //Mostra le informazioni del territorio
            gameManager.GetComponent<GameManagerUI>()
                .ShowTerritoryInfo(TerritoryInformationsAllPlayers(newTerritory.name));
            //Cambia il colore del testo in base al colore del giocatore per migliorare la leggibilità
            if (Player.Instance.ArmyColor.Equals("black") || Player.Instance.ArmyColor.Equals("blue")) {
                stateNameAddTank.color = Color.white;
                tankNumber.color = Color.white;
                tankToAdd.color = Color.white;
            }

            //Se il territorio è di un giocatore allora mostra il popup per aggiungere armate
            if (TerritoryInformationsPlayer(newTerritory.name) is not null) {
                stateNameAddTank.text = TerritoryInformationsPlayer(newTerritory.name).name;
                tankNumber.text = TerritoryInformationsPlayer(newTerritory.name).num_tanks + "";
                int result = FindTerritory(selectedTerritory.name);
                //Controlla se sono già stati aggiuntu altre armate in questa fase e rispristina tale numero nell'UI
                if (result == -1)
                    tankToAdd.text = 0 + "";
                else
                    tankToAdd.text = _selectedTerritories.count[result] + "";
                //Mostra il popup per aggiungere armate e lo posiziona sopra il territorio
                popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
                popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
                    popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
                popUpAddTank.SetActive(true);
            }
            else {
                popUpAddTank.SetActive(false);
                selectedTerritory.Deselect();
                selectedTerritory = null;
            }
        }

        public void DeselectState() {
            selectedTerritory.Deselect();
            selectedTerritory = null;
        }
    }
}