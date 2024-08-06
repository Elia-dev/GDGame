using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public struct SelectedTerritories {
    public List<Territory> territories;
    public int[] count;
}

public class TerritoriesManagerDistrPhaseUI : TerritoriesManagerUI {
    public static TerritoriesManagerDistrPhaseUI Instance { get; private set; }
    [SerializeField] private GameObject popUpAddTank;
    [SerializeField] private TMP_Text TankNumber;
    SelectedTerritories selectedTerritories;
    private bool isTurnActive = false; // Variabile per tracciare il turno attivo
    private bool isTurnInitialized = false; // Variabile per tracciare se il turno è stato inizializzato

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

        //FUORI DEBUG
        popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
        activateTerritories(Player.Instance.Territories);
    }

    public void activateTerritories(List<Territory> territories) {
        foreach (var territory in territories) {
            GameObject terr = this.territories.Find(x => x.name.Equals(territory.CardId));
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
        //ATTESA DEL TURNO
        //if(ricezione messaggio server)
        //StartTurn();
        //RICEZIONE NUMERO ARMATE DA POSIZIONE
        //int armyNumber = ...
        int armyNumber = 3;
        selectedTerritories.territories = new List<Territory>(armyNumber);
        selectedTerritories.count = new int[armyNumber];

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider is not null) {
                TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                if (territoryHandlerUI is not null) {
                    selectedTerritory = territoryHandlerUI;
                    SelectState(selectedTerritory);
                }
            }
        } /*else if (Input.GetMouseButtonDown(0) && !distributionPhase) {
            gamePhaseSelection();
        }
        /*if (Input.GetMouseButtonDown(1) && distributionPhase) {
            distributionPhaseDeselection();
        } else if (Input.GetMouseButtonDown(1) && !distributionPhase) {
        }*/
    }
    
    //Metodo che inizializza la struct per la selezione dei territori e attiva il turno
    public void StartTurn(int armyNumber) {
        selectedTerritories.territories = new List<Territory>(armyNumber);
        selectedTerritories.count = new int[armyNumber];
        isTurnActive = true; // Attiva il turno
        isTurnInitialized = false; // Imposta a false per inizializzare nel prossimo Update
        //Debug.Log("Turno iniziato con " + armyNumber + " armate da posizionare.");
    }

    /*IEnumerator distributionPhaseSelection() {
        yield return null;
    }

    public void gamePhaseSelection() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider is not null) {
            TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
            if (territoryHandlerUI is not null) {
                SelectState(territoryHandlerUI);
            }
        }
        else {
            DeselectState();
        }
    }*/

    Territory TerritoryInformations(string name) {
        return Player.Instance.Territories.Find(x => x.CardId.Equals(name));
    }

    private int selectTerritory(TerritoryHandlerUI territory) {
        for (int i = 0; i < selectedTerritories.count.Length; i++) {
            Debug.Log(selectedTerritories.count[i]);
        }

        if (selectedTerritories.count[0] +
            selectedTerritories.count[1] +
            selectedTerritories.count[2] < selectedTerritories.count.Length) {
            int result = selectedTerritories.territories.FindIndex(x => x.Name.Equals(territory.name));
            Debug.Log(result);
            if (result == -1) {
                for (int i = 0; i < selectedTerritories.count.Length; i++)
                    if (selectedTerritories.count[i] == 0) {
                        selectedTerritories.territories.Insert(i, TerritoryInformations(territory.name));
                        selectedTerritories.count[i]++;
                        Debug.Log(selectedTerritories.territories[i] + " " +
                                  selectedTerritories.count[i]);
                        return i;
                    }
            }
            else {
                selectedTerritories.count[result]++;
                return result;
            }
        }

        return -1;
    }

    public void SelectState(TerritoryHandlerUI newTerritory) {
        int result = selectTerritory(newTerritory);
        if (result != -1 && !newTerritory.Selected) {
            newTerritory.Select();
            TankNumber.text = TerritoryInformations(newTerritory.name).NumTanks + "+" +
                              selectedTerritories.count[result];
            popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
            popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
                popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
            popUpAddTank.SetActive(true);
        }
        /*else {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
            }
            selectedTerritory = newTerritory;
            selectedTerritory.Select();
        }*/
    }

    public void DeselectState() {
        if (distributionPhase)
            selectedTerritory.Deselect();
        else {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
                selectedTerritory = null;
            }
        }
    }
}