using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public struct SelectedTerritories {
    public Territory[] territories;
    public int[] count;
}

public class TerritoriesManagerDistrPhaseUI : TerritoriesManagerUI {
    public static TerritoriesManagerDistrPhaseUI Instance { get; private set; }
    [SerializeField] private GameObject popUpAddTank;
    [SerializeField] private TMP_Text stateNameAddTank;
    [SerializeField] private TMP_Text tankNumber;
    [SerializeField] private TMP_Text tankToAdd;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button endTurn;
    SelectedTerritories selectedTerritories;
    //private Dictionary<string, int> selectedTerritories;

    private bool isTurnActive = false; // Variabile per tracciare il turno attivo
    private bool isTurnInitialized = false; // Variabile per tracciare se il turno è stato inizializzato
    private int armyNumber;

    public void Start() {
        //TUTTA ROBA DI DEBUG
        /*TerritoryHandlerUI.userColor = new Color32(0, 0, 255, 200);
        List<Territory> terr = new List<Territory>();
        terr.Add(new Territory("SA_ter1", "SA_ter1.png", "boh", "eh", "lo", "fa", "SA", 7));
        terr.Add(new Territory("SA_ter2", "SA_ter2.png", "boh", "eh", "lo", "fa", "SA", 5));
        terr.Add(new Territory("SA_ter3", "SA_ter3.png", "boh", "eh", "lo", "fa", "SA", 6));
        terr.Add(new Territory("SA_ter4", "SA_ter4.png", "boh", "eh", "lo", "fa", "SA", 1));
        Player.Instance.Territories = terr;
        TerritoryHandlerUI.ArmyDistributionPhase();
        Player.Instance.IsMyTurn = true;
        Player.Instance.TanksAvailable = 3;

        //FUORI DEBUG
        //TerritoryHandlerUI.ArmyDistributionPhase();
        popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
        activateTerritories();*/
    }

    public void activateTerritories() {
        TerritoryHandlerUI.ArmyDistributionPhase();
        popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
        foreach (var territory in Player.Instance.Territories) {
            GameObject terr = base.territories.Find(x => x.name.Equals(territory.id));
            if (terr is not null) {
                terr.GetComponent<PolygonCollider2D>().enabled = true;
                Color32 color = TerritoryHandlerUI.userColor;
                color.a = 50;
                terr.GetComponent<SpriteRenderer>().color = color;
                terr.GetComponent<TerritoryHandlerUI>().StartColor = color;
            }
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

        plusButton.onClick.AddListener(() => AddArmy());
        minusButton.onClick.AddListener(() => RemoveArmy());
        endTurn.onClick.AddListener(() => SendArmy());
    }

    public void AddArmy() {
        int totalArmy = selectedTerritories.count.Sum();
        if (totalArmy < armyNumber) {
            int result = FindTerritory(selectedTerritory.name);

            if (result == -1) {
                for (int i = 0; i < selectedTerritories.count.Length; i++)
                    if (selectedTerritories.count[i] == 0) {
                        selectedTerritories.territories[i] = TerritoryInformations(selectedTerritory.name);
                        selectedTerritories.count[i]++;
                        tankToAdd.text = selectedTerritories.count[i] + "";
                        i = selectedTerritories.count.Length;
                    }
            }
            else {
                selectedTerritories.count[result]++;
                tankToAdd.text = selectedTerritories.count[result] + "";
            }

            selectedTerritory.Select();
            CheckTotalArmy();
        }
    }

    public void RemoveArmy() {
        if (int.Parse(tankToAdd.text) > 0) {
            int result = FindTerritory(selectedTerritory.name);

            selectedTerritories.count[result]--;
            tankToAdd.text = selectedTerritories.count[result] + "";
            if (selectedTerritories.count[result] == 0) {
                selectedTerritories.territories[result] = null;
                selectedTerritory.Deselect();
            }

            CheckTotalArmy();
        }
    }

    private void CheckTotalArmy() {
        if (selectedTerritories.count.Sum() == armyNumber)
            endTurn.interactable = true;
        else
            endTurn.interactable = false;
    }

    private void SendArmy() {
        for (int i = 0; i < armyNumber; i++) {
            Player.Instance.Territories.ForEach(terr => {
                if (selectedTerritories.territories[i] is not null &&
                    terr.id.Equals(selectedTerritories.territories[i].id)) {
                    terr.num_tanks += selectedTerritories.count[i];
                    base.territories.Find(obj => obj.name.Equals(terr.id)).GetComponent<TerritoryHandlerUI>()
                        .Deselect();
                }
            });
        }

        popUpAddTank.SetActive(false);
        Player.Instance.TanksAvailable -= selectedTerritories.count.Sum();
        ClientManager.Instance.UpdateTerritoriesState();
        endTurn.interactable = false;
        isTurnInitialized = false;
    }

    private int FindTerritory(string TerritoryName) {
        for (int i = 0; i < armyNumber; i++) {
            if (selectedTerritories.territories[i] is not null &&
                selectedTerritories.territories[i].id.Equals(TerritoryName))
                return i;
        }

        return -1;
    }

    private void Update() {
        //Debug.Log("IsMyTurn: " + Player.Instance.IsMyTurn);
        if (Player.Instance.IsMyTurn && !isTurnInitialized) {
            StartTurn();
        }

        if (Input.GetMouseButtonDown(0) && Player.Instance.IsMyTurn) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            RaycastHit2D hit = new RaycastHit2D();
            foreach (RaycastHit2D hitted in hits) {
                Collider2D collider = hitted.collider;

                if (collider is BoxCollider2D) {
                    hit = new RaycastHit2D();
                    break;
                }

                if (collider is PolygonCollider2D)
                    hit = hitted;
            }

            if (hit.collider is not null) {
                TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                if (territoryHandlerUI is not null) {
                    selectedTerritory = territoryHandlerUI;
                    SelectState(selectedTerritory);
                }
            }
        }

        if (GameManager.Instance.getGamePhase()) {
            this.GetComponent<TerritoriesManagerDistrPhaseUI>().enabled = false;
            this.GetComponent<TerritoriesManagerGamePhaseUI>().enabled = true;
            Debug.Log("Game Phase");
        }
    }

    //Metodo che inizializza la struct per la selezione dei territori e attiva il turno
    public void StartTurn() {
        isTurnInitialized = true; // Attiva il turno
        armyNumber = Player.Instance.TanksAvailable;
        if (armyNumber > 3) {
            armyNumber = 3;
        }

        selectedTerritories.territories = new Territory[armyNumber];
        selectedTerritories.count = new int[armyNumber];
    }

    Territory TerritoryInformations(string name) {
        return Player.Instance.Territories.Find(x => x.id.Equals(name));
    }

    //Mostra il popup per aggiungere o togliere armate
    public void SelectState(TerritoryHandlerUI newTerritory) {
        stateNameAddTank.text = TerritoryInformations(newTerritory.name).Name;
        tankNumber.text = TerritoryInformations(newTerritory.name).num_tanks + "";
        int result = FindTerritory(selectedTerritory.name);
        //Controlla se sono già stati aggiuntu altre armate in questa fase e rispristina tale numero
        if (result == -1)
            tankToAdd.text = 0 + "";
        else
            tankToAdd.text = selectedTerritories.count[result] + "";
        
        popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
        popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
            popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
        popUpAddTank.SetActive(true);
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