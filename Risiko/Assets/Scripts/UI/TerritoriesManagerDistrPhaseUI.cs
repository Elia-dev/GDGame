using System;
using System.Collections;
using System.Linq;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public struct SelectedTerritories {
        public Territory[] Territories;
        public int[] Count;
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
        private bool _isTurnInitialized = false;
        private bool _iAmAlive = true;

        public int ArmyNumber { get; private set; }

        private void Awake() {
            plusButton.onClick.AddListener(AddArmy);
            minusButton.onClick.AddListener(RemoveArmy);
            endTurnButton.onClick.AddListener(() => {
                if (distributionPhase || TerritoriesManagerGamePhaseUI.ReinforcePhase) {
                    SendArmy();
                }
                else if (TerritoriesManagerGamePhaseUI.AttackPhase) {
                    TerritoriesManagerGamePhaseUI.AttackPhase = false;
                    GameManagerUI.AttackPhase = false;
                    if (TerritoriesManagerGamePhaseUI.FirstTurn)
                        TerritoriesManagerGamePhaseUI.FirstTurn = false;
                    ClientManager.Instance.UpdateTerritoriesState();
                    endTurnButton.interactable = false;
                    StartCoroutine(WaitForTurnToEnd());
                }
            });
        }

        private IEnumerator WaitForTurnToEnd()
        {
            while (Player.Instance.IsMyTurn)
            {
                yield return null;
            }
            OnTurnEnded();
        }
        public void Start() {
            GameManagerUI.DistributionPhase = true;
        }

        public void ActivateTerritories() {
            popUpAddTank.GetComponent<Image>().color =
                Utils.ColorCode(Player.Instance.ArmyColor, 255);
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

        private void AddArmy() {
            int totalArmy = _selectedTerritories.Count.Sum();
            if (totalArmy < ArmyNumber) {
                int result = FindTerritory(SelectedTerritory.name);
                if (result == -1) {
                    for (int i = 0; i < _selectedTerritories.Count.Length; i++)
                        if (_selectedTerritories.Count[i] == 0) {
                            _selectedTerritories.Territories[i] = TerritoryInformationsPlayer(SelectedTerritory.name);
                            _selectedTerritories.Count[i]++;
                            tankToAdd.text = _selectedTerritories.Count[i] + "";
                            i = _selectedTerritories.Count.Length;
                        }
                }
                else {
                    _selectedTerritories.Count[result]++;
                    tankToAdd.text = _selectedTerritories.Count[result] + "";
                }
                SelectedTerritory.Select();
                CheckTotalArmy();
            }
        }

        private void RemoveArmy() {
            if (int.Parse(tankToAdd.text) > 0) {
                int result = FindTerritory(SelectedTerritory.name);

                _selectedTerritories.Count[result]--;
                tankToAdd.text = _selectedTerritories.Count[result] + "";
                if (_selectedTerritories.Count[result] == 0) {
                    _selectedTerritories.Territories[result] = null;
                    SelectedTerritory.Deselect();
                }

                CheckTotalArmy();
            }
        }

        private void CheckTotalArmy() {
            if (_selectedTerritories.Count.Sum() == ArmyNumber)
                endTurnButton.interactable = true;
            else
                endTurnButton.interactable = false;
        }

        private void SendArmy() {
            for (int i = 0; i < ArmyNumber; i++) {
                Player.Instance.Territories.ForEach(terr => {
                    if (_selectedTerritories.Territories[i] is not null &&
                        terr.id.Equals(_selectedTerritories.Territories[i].id)) {
                        terr.num_tanks += _selectedTerritories.Count[i];
                        base.territories.Find(obj => obj.name.Equals(terr.id))
                            .GetComponent<TerritoryHandlerUI>().Deselect();
                    }
                });
            }

            popUpAddTank.SetActive(false);
            Player.Instance.TanksAvailable -= _selectedTerritories.Count.Sum();
            Player.Instance.TanksPlaced += _selectedTerritories.Count.Sum();
            gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
            ClientManager.Instance.UpdateTerritoriesState();
            endTurnButton.interactable = false;
            _isTurnInitialized = false;

            if (!distributionPhase) {
                this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = false;
                TerritoriesManagerGamePhaseUI.ReinforcePhase = false;
                GameManagerUI.ReinforcePhase = false;
                TerritoriesManagerGamePhaseUI.AttackPhase = true;
                GameManagerUI.AttackPhase = true;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().IsPhaseGoing = false;
                endTurnButton.GetComponentInChildren<TMP_Text>().text = "End Turn!";
                endTurnButton.interactable = true;
            }
            this.GetComponent<TerritoriesManagerGamePhaseUI>().RefreshTerritories();
        }

        private int FindTerritory(string territoryName) {
            for (int i = 0; i < ArmyNumber; i++) {
                if (_selectedTerritories.Territories[i] is not null &&
                    _selectedTerritories.Territories[i].id.Equals(territoryName))
                    return i;
            }
            return -1;
        }

        private void Update() {
            if (!GameManager.Instance.GetGameRunning()) {
                GameManagerUI.ThisIsTheEnd = true;
                popUpPlayerLeftGame.SetActive(true);
                popUpPlayerLeftGame.GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("Player left the game\nyou will be redirected to the main menu...");
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
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
                RaycastHit2D hit = new RaycastHit2D();
                foreach (RaycastHit2D hitted in hits) {
                    Collider2D hittedCollider = hitted.collider;
                    if (hittedCollider is BoxCollider2D) {
                        hit = hitted;
                        break;
                    }
                    if (hittedCollider is PolygonCollider2D)
                        hit = hitted;
                }

                if (hit.collider is PolygonCollider2D) {
                    TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                    if (territoryHandlerUI is not null) {
                        SelectedTerritory = territoryHandlerUI;
                        SelectState(SelectedTerritory);
                    }
                }
                else if (hit.collider is null) {
                    gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                    popUpAddTank.SetActive(false);
                    if (SelectedTerritory is not null) {
                        SelectedTerritory = null;
                    }
                }
            }

            if (!Player.Instance.IsMyTurn) {
                if (Input.GetMouseButtonDown(0)) {
                    Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                    foreach (Canvas canvas in allCanvases) {
                        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
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

            if (GameManager.Instance.GetGamePhase() && distributionPhase) {
                TerritoriesManagerUI.distributionPhase = false;
                GameManagerUI.DistributionPhase = false;
                this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = false;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = true;
                this.GetComponent<TerritoriesManagerGamePhaseUI>().ActivateOtherPlayersTerritories();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }

                if (popUpAddTank.activeInHierarchy) {
                    //Debug.Log("Disattivo il popup AddTank");
                    popUpAddTank.SetActive(false);
                    if (SelectedTerritory is not null) SelectedTerritory = null;
                }
                else
                    escMenu.SetActive(true);
            }
            this.GetComponent<TerritoriesManagerGamePhaseUI>().RefreshTankNumbersFlags();
        }

        //Metodo che inizializza la struct per la selezione dei territori e attiva il turno
        public void StartTurn() {
            _isTurnInitialized = true;
            ArmyNumber = Player.Instance.TanksAvailable;
            if (ArmyNumber > 3 && distributionPhase) {
                ArmyNumber = 3;
            }

            _selectedTerritories.Territories = new Territory[ArmyNumber];
            _selectedTerritories.Count = new int[ArmyNumber];
        }

        private Territory TerritoryInformationsAllPlayers(string id) {
            return GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        }

        private void SelectState(TerritoryHandlerUI newTerritory) {
            gameManager.GetComponent<GameManagerUI>()
                .ShowTerritoryInfo(TerritoryInformationsAllPlayers(newTerritory.name));
            if (Player.Instance.ArmyColor.Equals("black") || Player.Instance.ArmyColor.Equals("blue")) {
                stateNameAddTank.color = Color.white;
                tankNumber.color = Color.white;
                tankToAdd.color = Color.white;
            }

            if (TerritoryInformationsPlayer(newTerritory.name) is not null) {
                stateNameAddTank.text = TerritoryInformationsPlayer(newTerritory.name).name;
                tankNumber.text = TerritoryInformationsPlayer(newTerritory.name).num_tanks + "";
                int result = FindTerritory(SelectedTerritory.name);
                if (result == -1)
                    tankToAdd.text = 0 + "";
                else
                    tankToAdd.text = _selectedTerritories.Count[result] + "";
                popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
                popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
                    popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
                popUpAddTank.SetActive(true);
            }
            else {
                popUpAddTank.SetActive(false);
                SelectedTerritory.Deselect();
                SelectedTerritory = null;
            }
        }
        
        private Territory TerritoryInformationsPlayer(string id) {
            return Player.Instance.Territories.Find(x => x.id.Equals(id));
        }

        private void OnTurnEnded()
        {
            // Codice da eseguire quando il turno è terminato
            TerritoriesManagerGamePhaseUI.IsTurnInitialized = false;
        }
    }
}