using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TerritoriesManagerDistrPhaseUI : MonoBehaviour {
    public static TerritoriesManagerDistrPhaseUI Instance { get; private set; }
    [FormerlySerializedAs("selectedCountry")] public TerritoryHandlerUI selectedTerritory;
    [SerializeField] public List<GameObject> territories;
    private bool distributionPhase = true;
    [SerializeField] private GameObject popUpAddTank;

    public void Start() {
        //TUTTA ROBA DI DEBUG
        TerritoryHandlerUI.userColor = new Color32(0, 0, 255, 100);
        List<Territory> terr = new List<Territory>();
        terr.Add(new Territory("SA_ter1", "SA_ter1.png", "boh", "eh", "lo", "fa", 14, "SA"));
        terr.Add(new Territory("SA_ter2", "SA_ter2.png", "boh", "eh", "lo", "fa", 14, "SA"));
        activateTerritories(terr);
        //activateTerritories(Player.Instance.Territories);
    }
    
    public void activateTerritories(List<Territory> territories) {
        foreach (var territory in territories) {
            GameObject terr = this.territories.Find(x => x.name.Equals(territory.cardId));
            //Debug.Log(terr.gameObject.name);
            if (terr is not null)
                terr.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }
    
    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && distributionPhase) {
            distributionPhaseSelection();
        } else if (Input.GetMouseButtonDown(0) && !distributionPhase) {
            gamePhaseSelection();
        }
    }

    public void distributionPhaseSelection() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider is not null) {
            TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
            if (territoryHandlerUI is not null) {
                selectedTerritory = territoryHandlerUI;
                SelectState(selectedTerritory);
            }
        }
    }

    public void gamePhaseSelection() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider is not null) {
            TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
            if (territoryHandlerUI is not null) {
                SelectState(territoryHandlerUI);
            }
        } else {
            DeselectState();
        }
    }

    Territory TerritoryInformations(string name) {
        return Player.Instance.Territories.Find(x => x.cardId.Equals(name));
    }
    public void SelectState(TerritoryHandlerUI newTerritory) {
        if (distributionPhase) {
            newTerritory.Select();
            popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
            GameObject.Find("TankNumber").GetComponent<TMP_Text>().text = TerritoryInformations(newTerritory.name).NumTanks.ToString();
            popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
            popUpAddTank.SetActive(true);
        }
        else {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
            }
            selectedTerritory = newTerritory;
            selectedTerritory.Select();
        }
    }

    public void DeselectState() {
        if(distributionPhase) 
            selectedTerritory.Deselect();
        else {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
                selectedTerritory = null;
            }
        }
    }
}