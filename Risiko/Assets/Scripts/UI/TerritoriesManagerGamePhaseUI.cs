using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI
{
    public static TerritoriesManagerGamePhaseUI Instance { get; private set; }
    private List<GameObject> _neighborhoodGameObj;
    private List<Territory> _neighborhoodTeeritories;
    private bool _reinforcePhase = true;
    private bool _attackphase = false;
    private bool _isTurnInitialized = false;
    public bool IsPhaseGoing { get; set; } = false;
    private float _delay = 5.0f; // Durata del ritardo in secondi
    private float _timer;

    public bool ReinforcePhase {
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
                Debug.Log("Reinforce phase");
                if (_timer > 0) 
                    _timer -= Time.deltaTime; // Decrementa il timer in base al tempo trascorso dall'ultimo frame
                else {
                    _timer = _delay;
                    IsPhaseGoing = true;
                    this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
                }
            }
            else {
                _reinforcePhase = false;
                _attackphase = true;
            }
        } else if (_attackphase && !IsPhaseGoing) {
            endTurnButton.enabled = true;
            ActivateOtherPlayersTerritories();
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider is not null) {
                    TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                    if (territoryHandlerUI is not null) {
                        selectedTerritory = territoryHandlerUI;
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

    private void ActivateOtherPlayersTerritories() {
        
    }
    
    private void StartTurn() {
        _isTurnInitialized = true;
        _timer = _delay;
        endTurnButton.GetComponentInChildren<TMP_Text>().text = "Next Phase!";
        //TODO
    }
    
    public void SelectState(TerritoryHandlerUI newTerritory) {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
        }
        selectedTerritory = newTerritory;
        selectedTerritory.Select();
        //Faccio apparire informazioni stato
        if(TerritoryInformationsPlayer(selectedTerritory.name) is not null)
            //Interrogazione server per ricevere la lista dei territori vicini
            //neighborhoodTeeritories = GameManager.Instance.SOMETHING;
            foreach (var territory in _neighborhoodTeeritories) {
                GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
                _neighborhoodGameObj.Add(terr);
                if (terr is not null) {
                    Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                    tempColor.a = 120;
                    terr.GetComponent<SpriteRenderer>().color = tempColor;
                }
            }
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
                Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                tempColor.a = 50;
                terr.GetComponent<SpriteRenderer>().color = tempColor;
            }
            selectedTerritory = null;
        }
    }
}
