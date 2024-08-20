using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI
{
    public static TerritoriesManagerGamePhaseUI Instance { get; private set; }
    [SerializeField] private GameObject popUpAttack;
    private List<GameObject> _neighborhoodGameObj = new List<GameObject>();
    private List<Territory> _neighborhoodTeeritories = new List<Territory>();
    public TerritoryHandlerUI enemyTerritory;
    private bool _reinforcePhase = true;
    private bool _attackphase = false;
    private bool _isTurnInitialized = false;
    private bool _readyToAttack = false;
    public bool IsPhaseGoing { get; set; } = false;
    public bool ReinforcePhase {
        get => _reinforcePhase;
        set => _reinforcePhase = value;
    }
    public bool Attackphase {
        set => _attackphase = value;
    }
    
    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
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
                _attackphase = true;
            }
        } else if (_attackphase && !IsPhaseGoing) {
            endTurnButton.enabled = true;
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider is not null) {
                    TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                    if (territoryHandlerUI is not null) {
                        //selectedTerritory = territoryHandlerUI;
                        SelectState(territoryHandlerUI);
                    }
                }
                else {
                    DeselectState();
                }
            }
        }
        /*else {
            endTurnButton.GetComponentInChildren<TMP_Text>().text = "End Turn!";

        }*/
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
        _isTurnInitialized = true;
        //_timer = _delay;
        endTurnButton.GetComponentInChildren<TMP_Text>().text = "Next Phase!";
        //TODO
    }
    
    public void SelectState(TerritoryHandlerUI newTerritory) {
        //Se ho selezionato un mio stato
        if (TerritoryInformationsPlayer(newTerritory.gameObject.name) is not null) {
            //Se ho già selezionato un mio stato e questo è confinante ad esso
            if (_readyToAttack && _neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                //POPUP MOVE
                Debug.Log("POPUP MOVE");
            }
            else { //Altrimenti ho selezionato un nuovo stato e quindi vado alla ricerca dei vicini
                //BRILLO I VICINI e debrillo quelli  di prima
                if (selectedTerritory is not null) {
                    Debug.Log("DESELECT AMICI");
                    selectedTerritory.Deselect();
                    DeselectState();
                }
                selectedTerritory = newTerritory;
                selectedTerritory.Select();
                _neighborhoodTeeritories =
                    Utils.GetNeighborsOf(TerritoryInformationsPlayer(selectedTerritory.gameObject.name));
                _neighborhoodGameObj = new List<GameObject>();
                _readyToAttack = true;
                foreach (var territory in _neighborhoodTeeritories) {
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
        else { //Se invece ho selezionato uno stato nemico
            //Se è nei dintorni del mio stato selezionato
            if (_readyToAttack && _neighborhoodGameObj.Contains(newTerritory.gameObject)) {
                enemyTerritory = newTerritory;
                popUpAttack.GetComponent<PopUpAttackUI>().SetPupUp(
                    TerritoryInformationsPlayer(selectedTerritory.gameObject.name), 
                    TerritoryInformationsOtherPLayers(enemyTerritory.gameObject.name), 
                    enemyTerritory.gameObject);
            }
            else { //Se invece non è nei dintorni 
                if (selectedTerritory is not null) {
                    Debug.Log("DESELECT AMICI");
                    selectedTerritory.Deselect();
                    selectedTerritory = null;
                    DeselectState();
                }
                if (enemyTerritory is not null) {
                    Debug.Log("DESELECT NEMICO");
                    enemyTerritory.Deselect();
                    enemyTerritory = null;
                }
                _neighborhoodGameObj = new List<GameObject>();
                _neighborhoodTeeritories = new List<Territory>();
                enemyTerritory = newTerritory;
                enemyTerritory.Select();
                //CARICO INFO
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
            _neighborhoodTeeritories =
                Utils.GetNeighborsOf(TerritoryInformationsPlayer(selectedTerritory.gameObject.name));
            foreach (var terr in _neighborhoodTeeritories) {
            }
            _neighborhoodGameObj = new List<GameObject>();
            foreach (var territory in _neighborhoodTeeritories) {
                _readyToAttack = true;
                GameObject terr = base.territories.Find(obj => obj.name.Equals(territory.id));
                if (terr is not null) {
                    _neighborhoodGameObj.Add(terr);
                    string color = GameManager.Instance.GetPlayerColor(territory.player_id);
                    terr.GetComponent<SpriteRenderer>().color = Utils.ColorCode(color, 120);
                    terr.GetComponent<TerritoryHandlerUI>().StartColor = Utils.ColorCode(color, 120);
                }
            }
        } else if (TerritoryInformationsOtherPLayers(selectedTerritory.name) is not null && _readyToAttack) {
            //popUpAttack.GetComponent<PopUpAttackUI>().SetPupUp(selectedTerritory, );
        }*/
    }

    Territory TerritoryInformationsPlayer(string id) {
        return Player.Instance.Territories.Find(x => x.id.Equals(id));
    }
    
    Territory TerritoryInformationsOtherPLayers(string id) {
        return Player.Instance.Territories.Find(x => x.id.Equals(id));
    }
    
    public void DeselectState() {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
            foreach (var terr in _neighborhoodGameObj) {
                Debug.Log("Deseleziono lo stato: " + terr);
                Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                tempColor.a = 50;
                Debug.Log("Colore: " + tempColor);
                terr.GetComponent<SpriteRenderer>().color = tempColor;
                terr.GetComponent<TerritoryHandlerUI>().StartColor = tempColor;
            }
            selectedTerritory = null;
            _neighborhoodGameObj = new List<GameObject>();
            _neighborhoodTeeritories = new List<Territory>();
            
            if (enemyTerritory is not null) {
                enemyTerritory.Deselect();
                enemyTerritory = null;
            }
        }
    }
}
