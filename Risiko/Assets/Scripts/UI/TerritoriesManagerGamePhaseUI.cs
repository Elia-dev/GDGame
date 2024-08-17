using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI
{
    public static TerritoriesManagerGamePhaseUI Instance { get; private set; }
    private List<GameObject> neighborhoodGameObj;
    private List<Territory> neighborhoodTeeritories;
    private bool reinforcePhase = true;
    private bool attackphase = false;
    private bool isTurnInitialized = false;
    private bool isPhaseGoing = false;

    public bool IsPhaseGoing {
        get => isPhaseGoing;
        set => isPhaseGoing = value;
    }

    public bool ReinforcePhase {
        set => reinforcePhase = value;
    }
    public bool Attackphase {
        set => attackphase = value;
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
        if (Player.Instance.IsMyTurn && !isTurnInitialized) {
            StartTurn();
        }
        
        if (reinforcePhase && !isPhaseGoing) {
            isPhaseGoing = true;
            this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
        } else if (attackphase && !isPhaseGoing) {
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
        else {
            endTurnButton.GetComponentInChildren<TMP_Text>().text = "End Turn!";
            
        }
    }

    private void ActivateOtherPlayersTerritories() {
        
    }
    
    private void StartTurn() {
        isTurnInitialized = true;
        endTurnButton.GetComponentInChildren<TMP_Text>().text = "Next Phase!";
        //TODO
    }
    
    public void SelectState(TerritoryHandlerUI newTerritory) {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
        }
        selectedTerritory = newTerritory;
        selectedTerritory.Select();
        //Interrogazione server per ricevere la lista dei territori vicini
        //neighborhoodTeeritories = GameManager.Instance.SOMETHING;
        foreach (var territory in neighborhoodTeeritories) {
            GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
            neighborhoodGameObj.Add(terr);
            if (terr is not null) {
                Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                tempColor.a = 120;
                terr.GetComponent<SpriteRenderer>().color = tempColor;
            }
        }
        
    }

    Territory TerritoryInformations(string id) {
        return Player.Instance.Territories.Find(x => x.id.Equals(id));
    }
    
    public void DeselectState() {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
            foreach (var terr in neighborhoodGameObj) {
                Color32 tempColor = terr.GetComponent<SpriteRenderer>().color;
                tempColor.a = 50;
                terr.GetComponent<SpriteRenderer>().color = tempColor;
            }
            selectedTerritory = null;
        }
    }
}
