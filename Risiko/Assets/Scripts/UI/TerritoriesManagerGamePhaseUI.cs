using System;
using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI {
        //public static TerritoriesManagerGamePhaseUI Instance { get; private set; }
        [SerializeField] private GameObject popUpAttack;
        [SerializeField] private GameObject popUpMoveTanks;
        [SerializeField] private GameObject gameManager;
        [SerializeField] private GameObject popUpAttackResult;
        [SerializeField] private GameObject endGame;
        [SerializeField] private GameObject tenArmyFlag;
        [SerializeField] private GameObject escMenu;
        
        private List<GameObject> _neighborhoodGameObj = new List<GameObject>();
        private List<Territory> _neighborhoodTerritories = new List<Territory>();
        [NonSerialized] private TerritoryHandlerUI _enemyTerritory;
        private static bool _reinforcePhase = false;
        private static bool _attackPhase = false;
        private static bool _isTurnInitialized = false;
        private static bool _strategicMove = false;
        private static bool _underAttack = false;
        private static bool _firstTurn = true;

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

        public bool IsPhaseGoing { get; set; } = false;

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
            /*if (Instance is null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }*/

            //endTurnButton.onClick.AddListener( () => ClientManager.Instance.UpdateTerritoriesState());
        }

        private void Start() {
            /*Player.Instance.IsMyTurn = true;
        distributionPhase = false;*/
        }

        private void Update() {
            if (Player.Instance.IsMyTurn && !_isTurnInitialized) {
                StartTurn();
            }

            if (_reinforcePhase && !IsPhaseGoing && Player.Instance.TanksAvailable > 0) {
                if (Player.Instance.Territories.Count >= 3) {
                    GameManagerUI.ReinforcePhase = true;
                    //if (_timer > 0) 
                    //_timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
                    //else {
                    //_timer = _delay;
                    IsPhaseGoing = true;
                    this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
                    //Debug.Log("Avvio altro script");
                    GetComponent<TerritoriesManagerDistrPhaseUI>().StartTurn();
                    //}
                }
                else {
                    _reinforcePhase = false;
                    GameManagerUI.ReinforcePhase = false;
                    _attackPhase = true;
                    GameManagerUI.AttackPhase = true;
                }
            }
            else if (_attackPhase && !IsPhaseGoing) {
                endTurnButton.interactable = true;
                if (Input.GetMouseButtonDown(0)) {
                    Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                    foreach (Canvas canvas in allCanvases)
                    {
                        // Controlla se il canvas è in modalità Screen Space - Overlay
                        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        {
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
                            hit = hitted;//new RaycastHit2D();
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
                    } else if (hit.collider is null) {
                        DeselectState();
                        gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                        popUpMoveTanks.SetActive(false);
                        popUpAttack.SetActive(false);
                    }
                }
            }

            if (!Player.Instance.IsMyTurn) {
                if (Input.GetMouseButtonDown(0)) {
                    Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                    foreach (Canvas canvas in allCanvases)
                    {
                        // Controlla se il canvas è in modalità Screen Space - Overlay
                        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        {
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
                            gameManager.GetComponent<GameManagerUI>().
                                ShowTerritoryInfo(TerritoryInformationsAllPlayers(territoryHandlerUI.gameObject.name));
                            //selectedTerritory = territoryHandlerUI;
                            //SelectState(territoryHandlerUI);
                        }
                    }
                    else {
                        gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                    }
                }
            }

            if (GameManager.Instance.getForceUpdateAfterAttack()) {
                RefreshTerritories();
                DeselectState();
                GameManager.Instance.setForceUpdateAfterAttack(false);
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
            }

            if (_strategicMove) {
                endTurnButton.interactable = false;
                _strategicMove = false;
                _attackPhase = false;
                GameManagerUI.AttackPhase = false;
                _isTurnInitialized = false;
                DeselectState();
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                RefreshTerritories();
            }

            if ((GameManager.Instance.getImUnderAttack() || GameManager.Instance.getImAttacking()) && !_underAttack) {
                _underAttack = true;
                Debug.Log("getImAttacking: " + GameManager.Instance.getImAttacking() + " getImUnderAttack: " 
                          + GameManager.Instance.getImUnderAttack() + " _underAttack: " + _underAttack);
                popUpAttackResult.GetComponent<PopUpAttackResultUI>().SetPupUp();
            }

            if (!GameManager.Instance.getWinnerGameId().Equals("")) {
                gameObject.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = false;
                endGame.GetComponent<EndGameUI>().SetPopUp(GameManager.Instance.getWinnerGameId());
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases)
                {
                    // Controlla se il canvas è in modalità Screen Space - Overlay
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        // Controlla se il Canvas è attivo e se ha GameObject attivi
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                
                if(popUpAttack.activeInHierarchy || popUpMoveTanks.activeInHierarchy) {
                    popUpAttack.SetActive(false);
                    popUpMoveTanks.SetActive(false);
                } else if (selectedTerritory is not null) {
                    DeselectState();
                } else
                    escMenu.SetActive(true);
            }
        }

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
        public void PlaceFlags(PolygonCollider2D polygonCollider, Territory territory)
        {
            int numFlags = Mathf.Min(territory.num_tanks / 10, 3); // Calcola il numero di bandierine (massimo 3)
            Vector2[] flagPositions = CalculateFlagPositions(polygonCollider, numFlags);

            for (int i = 0; i < numFlags; i++)
            {
                GameObject flag = Instantiate(tenArmyFlag, polygonCollider.transform);
                flag.GetComponent<SpriteRenderer>().sprite = LoadSprite("Army/TenArmy" + 
                                                                        GameManager.Instance.GetPlayerColor(territory.player_id));
                flag.transform.localScale = new Vector3(0.15f, 0.15f, flag.transform.localScale.z);
                flag.transform.position = flagPositions[i];
                flag.transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y, polygonCollider.transform.position.z);
            }
        }

        private Vector2[] CalculateFlagPositions(PolygonCollider2D polygonCollider, int numFlags)
        {
            Vector2 center = CalculatePolygonCenter(polygonCollider);
            Vector2[] positions = new Vector2[numFlags];

            float angleStep = 360f / numFlags;
            for (int i = 0; i < numFlags; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f; // Offset per posizionare le bandierine attorno al centro
                positions[i] = center + offset;
                positions[i] = polygonCollider.transform.TransformPoint(positions[i]);
            }

            return positions;
        }

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
            gameManager.GetComponent<GameManagerUI>().
                ShowTerritoryInfo(TerritoryInformationsAllPlayers(newTerritory.gameObject.name));
            //Se ho selezionato un mio stato
            if (TerritoryInformationsPlayer(newTerritory.gameObject.name) is not null) {
                //Se ho già selezionato un mio stato e questo è confinante ad esso
                if (_neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                    if(popUpAttack.activeInHierarchy)
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
                
                    if(popUpMoveTanks.activeInHierarchy)
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
            /*
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
        }
        if(TerritoryInformationsPlayer(selectedTerritory.gameObject.name) is not null) {
            selectedTerritory = newTerritory;
            selectedTerritory.Select();
        }
        //Faccio apparire informazioni stato barra dx
        if (TerritoryInformationsPlayer(selectedTerritory.name) is not null) {
            //Interrogazione server per ricevere la lista dei territori vicini
            _neighborhoodTerritories =
                Utils.GetNeighborsOf(TerritoryInformationsPlayer(selectedTerritory.gameObject.name));
            foreach (var terr in _neighborhoodTerritories) {
            }
            _neighborhoodGameObj = new List<GameObject>();
            foreach (var territory in _neighborhoodTerritories) {
                _readyToAttack = true;
                GameObject terr = base.territories.Find(obj => obj.name.Equals(territory.id));
                if (terr is not null) {
                    _neighborhoodGameObj.Add(terr);
                    string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                    terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 120);
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 120);
                }
            }
        } else if (TerritoryInformationsAllPlayers(selectedTerritory.name) is not null && _readyToAttack) {
            //popUpAttack.GetComponent<PopUpAttackUI>().SetPupUp(selectedTerritory, );
        }*/
        }

        private Territory TerritoryInformationsPlayer(string id) {
            return Player.Instance.Territories.Find(terr => terr.id.Equals(id));
        }

        private Territory TerritoryInformationsAllPlayers(string id) {
            return GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        }

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