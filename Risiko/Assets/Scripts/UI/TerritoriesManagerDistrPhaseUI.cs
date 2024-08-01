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
    [SerializeField] private TMP_Text TankNumber;

    public void Start() {
        //TUTTA ROBA DI DEBUG
        TerritoryHandlerUI.userColor = new Color32(0, 0, 255, 150);
        List<Territory> terr = new List<Territory>();
        terr.Add(new Territory("SA_ter1", "SA_ter1.png", "boh", "eh", "lo", "fa", 7, "SA"));
        terr.Add(new Territory("SA_ter2", "SA_ter2.png", "boh", "eh", "lo", "fa", 5, "SA"));
        terr.Add(new Territory("SA_ter3", "SA_ter3.png", "boh", "eh", "lo", "fa", 6, "SA"));
        terr.Add(new Territory("SA_ter4", "SA_ter4.png", "boh", "eh", "lo", "fa", 1, "SA"));
        Player.Instance.Territories = terr;
        TerritoryHandlerUI.ArmyDistributionPhase();
        activateTerritories(Player.Instance.Territories);
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
        /*if (Input.GetMouseButtonDown(1) && distributionPhase) {
            distributionPhaseDeselection();
        } else if (Input.GetMouseButtonDown(1) && !distributionPhase) {
        }*/
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

    public void distributionPhaseDeselection() {
        DeselectState();
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
        if (distributionPhase && !selectedTerritory.Selected) {
            selectedTerritory.Select();
            popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
            Debug.Log(TerritoryInformations(newTerritory.name).NumTanks.ToString());
            TankNumber.text = TerritoryInformations(newTerritory.name).NumTanks.ToString();
            popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
            popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
                popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
            popUpAddTank.SetActive(true);
        } else if (distributionPhase && !selectedTerritory.Selected) {
            
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