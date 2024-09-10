using System;
using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;

namespace UI {
    public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI {
        [SerializeField] private GameObject popUpAttack;
        [SerializeField] private GameObject popUpMoveTanks;
        [SerializeField] private GameObject gameManager;
        [SerializeField] private GameObject popUpAttackResult;
        [SerializeField] private GameObject endGame;
        [SerializeField] private GameObject tenArmyFlag;
        [SerializeField] private GameObject escMenu;
        [SerializeField] private GameObject popUpPlayerLeftGame;
        [SerializeField] private Button xPopUpLeftGame;

        private List<GameObject> _neighborhoodGameObj = new List<GameObject>();
        private List<Territory> _neighborhoodTerritories = new List<Territory>();
        [NonSerialized] private TerritoryHandlerUI _enemyTerritory;
        private static bool _reinforcePhase = false;
        private static bool _attackPhase = false;
        private static bool _isTurnInitialized = false;
        private bool _isPhaseGoing = false;
        private static bool _strategicMove = false;
        private static bool _underAttack = false;
        private static bool _firstTurn = true;
        private static bool _iAmAlive = true;

        public static bool FirstTurn {
            get => _firstTurn;
            set => _firstTurn = value;
        }

        public static bool UnderAttack {
            get => _underAttack;
            set => _underAttack = value;
        }

        public static bool IsTurnInitialized {
            get => _isTurnInitialized;
            set => _isTurnInitialized = value;
        }

        public bool IsPhaseGoing {
            get => _isPhaseGoing;
            set => _isPhaseGoing = value;
        }

        public static bool ReinforcePhase {
            get => _reinforcePhase;
            set => _reinforcePhase = value;
        }

        public static bool AttackPhase {
            get => _attackPhase;
            set => _attackPhase = value;
        }

        public static bool StategicMove {
            get => _strategicMove;
            set => _strategicMove = value;
        }

        private void Awake() {
            xPopUpLeftGame.onClick.AddListener(() => {
                // Quando si preme il tasto X del popup si ritorna al menu principale se il gioco è finito
                // Altrimenti torno al gioco chiudendo il popup
                if (GameManager.Instance.GetGameRunning()) {
                    popUpPlayerLeftGame.SetActive(false);
                }
                else {
                    Player.Instance.ResetPlayer();
                    GameManager.Instance.ResetGameManager();
                    ClientManager.Instance.ResetConnection();
                    popUpPlayerLeftGame.SetActive(false);
                    SceneManager.LoadScene("MainMenu");
                }
            });
        }

        private void Update() {
            if (!GameManager.Instance.GetGameRunning()) {
                popUpPlayerLeftGame.SetActive(true);
                GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("Player left the game\nyou will be redirected to the main menu...");
            }

            if (!GameManager.Instance.getKillerId().Equals("") && _iAmAlive) {
                _iAmAlive = false;
                popUpPlayerLeftGame.SetActive(true);
                // Perché la riga sotto mi ha dato nullReferenceException?
                GameObject.Find("PopUpContainer").GetComponent<DisplayMessageOnPopUpUI>()
                    .SetErrorText("You have been destroyed by "
                                  + GameManager.Instance.getEnemyNameById(GameManager.Instance.getKillerId())
                                  + "!\nYOU HAVE BEEN DEFEATED, now you will be a spectator of a world in which you no longer have influence...");
            }

            if (Player.Instance.IsMyTurn && !_isTurnInitialized) {
                StartTurn();
            }

            //Se è il mio turno e non è in corso nessuna fase e ho dei carri armati da piazzare
            //Inizio la fase di rinforzo abilitando lo script TerritoriesManagerDistrPhaseUI
            if (_reinforcePhase && !IsPhaseGoing)
            {
                Debug.Log
                    ("Inizio la fase di rinforzo");
                if (Player.Instance.TanksAvailable > 0 || Player.Instance.Territories.Count >= 3) {
                    Debug.Log("Ho carri da piazzare");
                    GameManagerUI.ReinforcePhase = true;
                    IsPhaseGoing = true;
                    this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
                    GetComponent<TerritoriesManagerDistrPhaseUI>().StartTurn();
                }
                else {
                    Debug.Log("Non ho carri da piazzare");
                    //Se invece non ho carri da piazzare paso direttamente alla fase di attacco
                    ClientManager.Instance.UpdateTerritoriesState();
                    _reinforcePhase = false;
                    GameManagerUI.ReinforcePhase = false;
                    _attackPhase = true;
                    GameManagerUI.AttackPhase = true;
                }
                
            }
            else if (_attackPhase && !IsPhaseGoing) {
                
                Debug.Log("Inizio la fase di attacco");
                // Se sono in fase di attacco catturo i click sugli stati e mostro le informazioni
                endTurnButton.interactable = true;
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
                            //selectedTerritory = territoryHandlerUI;
                            SelectState(territoryHandlerUI);
                        }
                    }
                    else if (hit.collider is null) {
                        //Se clicco fuori da uno stato deseleziono tutto e nascondo le informazioni
                        DeselectState();
                        gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                        popUpMoveTanks.SetActive(false);
                        popUpAttack.SetActive(false);
                    }
                }
            }

            // Se non è il mio turno mostro SOLO le informazioni degli stati
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

            // Refresh necessario dopo attacco
            if (GameManager.Instance.getForceUpdateAfterAttack()) {
                RefreshTerritories();
                DeselectState();
                GameManager.Instance.setForceUpdateAfterAttack(false);
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
            }

            //Dpopo aver effettuato uno spostamento strategico fa finire il turno e aggiorna stato e  informazioni
            if (_strategicMove) {
                _attackPhase = false;
                GameManagerUI.AttackPhase = false;
                endTurnButton.interactable = false;
                if (_firstTurn)
                    _firstTurn = false;
                _strategicMove = false;
                _isTurnInitialized = false;
                DeselectState();
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                RefreshTerritories();
            }

            //Se sono sotto attacco o sto attaccando mostro il popup
            if ((GameManager.Instance.getImUnderAttack() || GameManager.Instance.getImAttacking()) && !_underAttack) {
                _underAttack = true;
                popUpAttackResult.GetComponent<PopUpAttackResultUI>().SetPupUp();
            }

            // Se il gioco è finito mostro il popup con vincitore/perdente
            if (!GameManager.Instance.getWinnerGameId().Equals("")) {
                gameObject.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = false;
                endGame.GetComponent<EndGameUI>().SetPopUp(GameManager.Instance.getWinnerGameId());
            }

            // Se premo ESC mostro il menu di pausa o chiudo i popup o deseleziono gli stati
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

                if (popUpAttack.activeInHierarchy || popUpMoveTanks.activeInHierarchy) {
                    popUpAttack.SetActive(false);
                    popUpMoveTanks.SetActive(false);
                }
                else if (selectedTerritory is not null) {
                    DeselectState();
                }
                else
                    escMenu.SetActive(true);
            }
        }

        //Funzione che aggiorna i colori degli stati e le bandierine
        public void RefreshTerritories() {
            foreach (var territory in GameManager.Instance.AllTerritories) {
                GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
                if (terr is not null) {
                    string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                    terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 50);
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 50);

                    //Distruggo tutte le precedenti bandierine
                    foreach (Transform child in terr.GetComponent<Transform>()) {
                        Destroy(child.gameObject);
                    }

                    PlaceFlags(terr.GetComponent<PolygonCollider2D>(), territory);
                }
            }
        }

        //Posiziona le bandierine negli stati
        public void PlaceFlags(PolygonCollider2D polygonCollider, Territory territory) {
            int numFlags = Mathf.Min(territory.num_tanks / 10, 3); // Calcola il numero di bandierine (massimo 3)
            Vector2[] flagPositions = CalculateFlagPositions(polygonCollider, numFlags);

            for (int i = 0; i < numFlags; i++) {
                GameObject flag = Instantiate(tenArmyFlag, polygonCollider.transform);
                flag.GetComponent<SpriteRenderer>().sprite = LoadSprite("Army/TenArmy" +
                                                                        GameManager.Instance.GetPlayerColor(
                                                                            territory.player_id));
                flag.transform.localScale = new Vector3(0.15f, 0.15f, flag.transform.localScale.z);
                flag.transform.position = flagPositions[i];
                flag.transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y,
                    polygonCollider.transform.position.z);
            }
        }

        //Calcola le posizioni delle bandierine
        private Vector2[] CalculateFlagPositions(PolygonCollider2D polygonCollider, int numFlags) {
            Vector2 center = CalculatePolygonCenter(polygonCollider);
            Vector2[] positions = new Vector2[numFlags];

            float angleStep = 360f / numFlags;
            for (int i = 0; i < numFlags; i++) {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 offset =
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) *
                    0.5f; // Offset per posizionare le bandierine attorno al centro
                positions[i] = center + offset;
                positions[i] = polygonCollider.transform.TransformPoint(positions[i]);
            }

            return positions;
        }

        //Calcola il centro dello stato basandosi sul PolygonCollider2D
        private Vector2 CalculatePolygonCenter(PolygonCollider2D polygonCollider) {
            Vector2[] points = polygonCollider.points;
            Vector2 sum = Vector2.zero;

            foreach (Vector2 point in points) {
                sum += point;
            }

            return sum / points.Length;
        }

        public Sprite LoadSprite(string spriteName) {
            return Resources.Load<Sprite>(spriteName);
        }

        //Attiva i territori degli altri giocatori
        public void ActivateOtherPlayersTerritories() {
            foreach (var territory in GameManager.Instance.AllTerritories) {
                if (!territory.player_id.Equals(Player.Instance.PlayerId)) {
                    GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
                    if (terr is not null) {
                        terr.GetComponent<PolygonCollider2D>().enabled = true;
                        string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                        terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 50);
                        terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 50);
                    }
                }
            }
        }

        //Setta le impostazioni iniziali del turno
        private void StartTurn() {
            RefreshTerritories();
            _isTurnInitialized = true;
            if (_firstTurn)
                _attackPhase = true;
            else {
                _reinforcePhase = true;
                endTurnButton.GetComponentInChildren<TMP_Text>().text = "Next Phase!";
            }
        }

        public void SelectState(TerritoryHandlerUI newTerritory) {
            //Info stato
            gameManager.GetComponent<GameManagerUI>()
                .ShowTerritoryInfo(TerritoryInformationsAllPlayers(newTerritory.gameObject.name));
            //Se ho selezionato un mio stato
            if (TerritoryInformationsPlayer(newTerritory.gameObject.name) is not null) {
                //Se ho già selezionato un mio stato e questo è confinante ad esso
                if (_neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                    if (popUpAttack.activeInHierarchy)
                        popUpAttack.SetActive(false);

                    popUpMoveTanks.GetComponent<PupUpMoveTanksUI>().SetPupUp(
                        TerritoryInformationsPlayer(selectedTerritory.gameObject.name),
                        TerritoryInformationsPlayer(newTerritory.gameObject.name),
                        newTerritory.gameObject);
                }
                else {
                    //Altrimenti ho selezionato un nuovo stato e quindi vado alla ricerca dei vicini
                    //BRILLO I VICINI e debrillo quelli  di prima
                    popUpMoveTanks.SetActive(false);
                    popUpAttack.SetActive(false);
                    DeselectState();
                    selectedTerritory = newTerritory;
                    selectedTerritory.Select();
                    _neighborhoodTerritories =
                        Utils.GetNeighborsOf(TerritoryInformationsPlayer(selectedTerritory.gameObject.name));
                    _neighborhoodGameObj = new List<GameObject>();
                    foreach (var territory in _neighborhoodTerritories) {
                        GameObject terr = base.territories.Find(obj => obj.name.Equals(territory.id));
                        if (terr is not null) {
                            _neighborhoodGameObj.Add(terr);
                            string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                            terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 150);
                            terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 150);
                        }
                    }
                }
            }
            else {
                //Se invece ho selezionato uno stato nemico
                //Se è nei dintorni del mio stato selezionato
                if (_neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                    //_readyToAttack &&
                    _enemyTerritory = newTerritory;

                    if (popUpMoveTanks.activeInHierarchy)
                        popUpMoveTanks.SetActive(false);

                    popUpAttack.GetComponent<PopUpAttackUI>().SetPupUp(
                        TerritoryInformationsPlayer(selectedTerritory.gameObject.name),
                        TerritoryInformationsAllPlayers(_enemyTerritory.gameObject.name),
                        _enemyTerritory.gameObject);
                }
                else {
                    //Se invece non è nei dintorni 
                    DeselectState();
                    popUpMoveTanks.SetActive(false);
                    popUpAttack.SetActive(false);
                    _neighborhoodGameObj = new List<GameObject>();
                    _neighborhoodTerritories = new List<Territory>();
                    _enemyTerritory = newTerritory;
                    _enemyTerritory.Select();
                }
            }
        }

        //Funzione che ritorna le informazioni di uno stato del giocatore
        private Territory TerritoryInformationsPlayer(string id) {
            return Player.Instance.Territories.Find(terr => terr.id.Equals(id));
        }

        //Funzione che ritorna le informazioni di uno stato di tutti i giocatori
        private Territory TerritoryInformationsAllPlayers(string id) {
            return GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        }

        //Deseleziona gli stati
        private void DeselectState() {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
                foreach (var terr in _neighborhoodGameObj) {
                    Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                    tempColor.a = 50;
                    terr.GetComponent<SpriteRenderer>().color = tempColor;
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = tempColor;
                }

                selectedTerritory = null;
                _neighborhoodGameObj = new List<GameObject>();
                _neighborhoodTerritories = new List<Territory>();
            }

            if (_enemyTerritory is not null) {
                _enemyTerritory.Deselect();
                _enemyTerritory = null;
            }
        }
    }
}