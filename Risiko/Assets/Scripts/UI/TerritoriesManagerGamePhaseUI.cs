using System;
using System.Collections;
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
        private static bool _strategicMove = false;
        private static bool _underAttack = false;
        private static bool _firstTurn = true;
        private static bool _iAmAlive = true;

        public static bool FirstTurn {
            get => _firstTurn;
            set => _firstTurn = value;
        }

        public static bool UnderAttack {
            set => _underAttack = value;
        }

        public static bool IsTurnInitialized {
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

        public static bool StrategicMove {
            get => _strategicMove;
            set => _strategicMove = value;
        }

        private void Awake() {
            xPopUpLeftGame.onClick.AddListener(() => {
                if (GameManager.Instance.GetGameRunning()) {
                    popUpPlayerLeftGame.SetActive(false);
                }
                else {
                    ClientManager.Instance.LeaveGame();
                    Player.Instance.ResetPlayer();
                    GameManager.Instance.ResetGameManager();
                    ClientManager.Instance.ResetConnection();
                    popUpPlayerLeftGame.SetActive(false);
                    SceneManager.LoadScene("MainMenu");
                }
            });
        }

        private void Start() {
            _reinforcePhase = false;
            _attackPhase = false;
            _isTurnInitialized = false;
            _strategicMove = false;
            _underAttack = false;
            _firstTurn = true;
            _iAmAlive = true;
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


            if (_reinforcePhase && !IsPhaseGoing)
            {
                if (Player.Instance.TanksAvailable > 0) {
                    GameManagerUI.ReinforcePhase = true;
                    IsPhaseGoing = true;
                    this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
                    GetComponent<TerritoriesManagerDistrPhaseUI>().StartTurn();
                }
                else {
                    ClientManager.Instance.UpdateTerritoriesState();
                    _reinforcePhase = false;
                    GameManagerUI.ReinforcePhase = false;
                    _attackPhase = true;
                    GameManagerUI.AttackPhase = true;
                    endTurnButton.GetComponentInChildren<TMP_Text>().text = "End Turn!";
                    endTurnButton.interactable = true;
                }
            }
            else if (_attackPhase && !IsPhaseGoing) {
                endTurnButton.interactable = true;
                if (Input.GetMouseButtonDown(0)) {
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
                            SelectState(territoryHandlerUI);
                        }
                    }
                    else if (hit.collider is null) {
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

            if (GameManager.Instance.GetForceUpdateAfterAttack()) {
                RefreshTerritories();
                DeselectState();
                GameManager.Instance.SetForceUpdateAfterAttack(false);
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
            }

            if (_strategicMove) {
                _attackPhase = false;
                GameManagerUI.AttackPhase = false;
                endTurnButton.interactable = false;
                if (_firstTurn)
                    _firstTurn = false;
                _strategicMove = false;
                StartCoroutine(WaitForTurnToEnd());
                DeselectState();
                gameManager.GetComponent<GameManagerUI>().HideTerritoryInfo();
                RefreshTerritories();
            }

            if ((GameManager.Instance.GetImUnderAttack() || GameManager.Instance.GetImAttacking()) && !_underAttack) {
                _underAttack = true;
                popUpAttackResult.GetComponent<PopUpAttackResultUI>().SetPupUp();
            }

            if (!GameManager.Instance.GetWinnerGameId().Equals("")) {
                gameObject.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = false;
                endGame.GetComponent<EndGameUI>().SetPopUp(GameManager.Instance.GetWinnerGameId());
            }

            // Se premo ESC mostro il menu di pausa o chiudo i popup o deseleziono gli stati
            if (Input.GetKeyDown(KeyCode.Escape) && !_reinforcePhase) {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases) {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }

                if (popUpAttack.activeInHierarchy || popUpMoveTanks.activeInHierarchy) {
                    popUpAttack.SetActive(false);
                    popUpMoveTanks.SetActive(false);
                }
                else if (SelectedTerritory is not null) {
                    DeselectState();
                }
                else
                    escMenu.SetActive(true);
            }
        }

        private void StartTurn() {
            Debug.Log("StartTurn Game phase; MyTurn: " + Player.Instance.IsMyTurn);
            RefreshTerritories();
            _isTurnInitialized = true;
            if (_firstTurn) {
                _attackPhase = true;
                GameManagerUI.AttackPhase = true; // Per barra dx
            }
            else {
                _reinforcePhase = true;
                endTurnButton.GetComponentInChildren<TMP_Text>().text = "Next Phase!";
            }
        }

        private void SelectState(TerritoryHandlerUI newTerritory) {
            gameManager.GetComponent<GameManagerUI>()
                .ShowTerritoryInfo(TerritoryInformationsAllPlayers(newTerritory.gameObject.name));
            if (TerritoryInformationsPlayer(newTerritory.gameObject.name) is not null) {
                if (_neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                    if (popUpAttack.activeInHierarchy)
                        popUpAttack.SetActive(false);
                    popUpMoveTanks.GetComponent<PupUpMoveTanksUI>().SetPupUp(
                        TerritoryInformationsPlayer(SelectedTerritory.gameObject.name),
                        TerritoryInformationsPlayer(newTerritory.gameObject.name),
                        newTerritory.gameObject);
                }
                else {
                    popUpMoveTanks.SetActive(false);
                    popUpAttack.SetActive(false);
                    DeselectState();
                    SelectedTerritory = newTerritory;
                    SelectedTerritory.Select();
                    _neighborhoodTerritories =
                        Utils.GetNeighborsOf(TerritoryInformationsPlayer(SelectedTerritory.gameObject.name));
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
                if (_neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                    _enemyTerritory = newTerritory;
                    if (popUpMoveTanks.activeInHierarchy)
                        popUpMoveTanks.SetActive(false);
                    popUpAttack.GetComponent<PopUpAttackUI>().SetPupUp(
                        TerritoryInformationsPlayer(SelectedTerritory.gameObject.name),
                        TerritoryInformationsAllPlayers(_enemyTerritory.gameObject.name),
                        _enemyTerritory.gameObject);
                }
                else {
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

        public void PlaceFlags(PolygonCollider2D polygonCollider, Territory territory) {
            int numFlags = Mathf.Min(territory.num_tanks / 10, 3);
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

        private Sprite LoadSprite(string spriteName) {
            return Resources.Load<Sprite>(spriteName);
        }

        private Vector2[] CalculateFlagPositions(PolygonCollider2D polygonCollider, int numFlags) {
            Vector2 center = CalculatePolygonCenter(polygonCollider);
            Vector2[] positions = new Vector2[numFlags];

            float angleStep = 360f / numFlags;
            for (int i = 0; i < numFlags; i++) {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 offset =
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f;
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

        private Territory TerritoryInformationsPlayer(string id) {
            return Player.Instance.Territories.Find(terr => terr.id.Equals(id));
        }

        private Territory TerritoryInformationsAllPlayers(string id) {
            return GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        }

        private void DeselectState() {
            if (SelectedTerritory is not null) {
                SelectedTerritory.Deselect();
                foreach (var terr in _neighborhoodGameObj) {
                    Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                    tempColor.a = 50;
                    terr.GetComponent<SpriteRenderer>().color = tempColor;
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = tempColor;
                }

                SelectedTerritory = null;
                _neighborhoodGameObj = new List<GameObject>();
                _neighborhoodTerritories = new List<Territory>();
            }

            if (_enemyTerritory is not null) {
                _enemyTerritory.Deselect();
                _enemyTerritory = null;
            }
        }

        private IEnumerator WaitForTurnToEnd() {
            while (Player.Instance.IsMyTurn) {
                yield return null;
            }
            OnTurnEnded();
        }

        private void OnTurnEnded() {
            _isTurnInitialized = false;
        }
    }
}