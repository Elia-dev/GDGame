using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI
{
    public static TerritoriesManagerGamePhaseUI Instance { get; private set; }
    private bool reinforcePhase = true;
    private bool attackphase = false;
    private bool isTurnInitialized = false;
    private bool isTurnGoing = false;

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
        
        if (reinforcePhase && !isTurnGoing) {
            isTurnGoing = true;
            this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = true;
        } else if (attackphase) {
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
    }

    public void DeselectState() {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
            selectedTerritory = null;
        }
    }
}
