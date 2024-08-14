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
    [SerializeField] private TMP_Text stateNameAddTank;
    [SerializeField] private TMP_Text tankNumber;
    [SerializeField] private TMP_Text tankToAdd;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    SelectedTerritories selectedTerritories;
    private bool isTurnActive = false; // Variabile per tracciare il turno attivo
    private bool isTurnInitialized = false; // Variabile per tracciare se il turno è stato inizializzato
    private int armyNumber;

    public void Start() {
        //TUTTA ROBA DI DEBUG
        /*TerritoryHandlerUI.userColor = new Color32(0, 0, 255, 150);
        List<Territory> terr = new List<Territory>();
        terr.Add(new Territory("SA_ter1", "SA_ter1.png", "boh", "eh", "lo", "fa", 7, "SA"));
        terr.Add(new Territory("SA_ter2", "SA_ter2.png", "boh", "eh", "lo", "fa", 5, "SA"));
        terr.Add(new Territory("SA_ter3", "SA_ter3.png", "boh", "eh", "lo", "fa", 6, "SA"));
        terr.Add(new Territory("SA_ter4", "SA_ter4.png", "boh", "eh", "lo", "fa", 1, "SA"));
        Player.Instance.Territories = terr;
        TerritoryHandlerUI.ArmyDistributionPhase();
        Player.Instance.IsMyTurn = true;*/
        int i = 0;
        foreach (var terri in Player.Instance.Territories) {
            Debug.Log("Territorio " + i + ": " + terri.id);
            i++;
        }
        
        //FUORI DEBUG
        TerritoryHandlerUI.ArmyDistributionPhase();
        popUpAddTank.GetComponent<Image>().color = TerritoryHandlerUI.userColor;
        activateTerritories(Player.Instance.Territories);
    }

    public void activateTerritories(List<Territory> territories) {
        foreach (var territory in territories) {
            GameObject terr = this.territories.Find(x => x.name.Equals(territory.id));
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
        Debug.Log("IsMyTurn: " + Player.Instance.IsMyTurn);
        if (Player.Instance.IsMyTurn && !isTurnInitialized) {
            armyNumber = Player.Instance.TanksAvailable;
            StartTurn(armyNumber);
        }

        //ATTESA DEL TURNO -> Player.Instance.IsMyTurn()
        //if(ricezione messaggio server)
        //StartTurn();
        //RICEZIONE NUMERO ARMATE DA POSIZIONE
        //int armyNumber = Player.Instance.TanksAvailable
        //int armyNumber = 3;
        //selectedTerritories.territories = new List<Territory>(armyNumber);
        //selectedTerritories.count = new int[armyNumber];

        if (Input.GetMouseButtonDown(0) && Player.Instance.IsMyTurn) {
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
        Debug.Log("StartTurn");
        isTurnInitialized = true; // Attiva il turno
        selectedTerritories.territories = new List<Territory>(armyNumber);
        selectedTerritories.count = new int[armyNumber];
        //isTurnInitialized = false; // Imposta a false per inizializzare nel prossimo Update
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
        return Player.Instance.Territories.Find(x => x.id.Equals(name));
    }

    ///funzione per chiedere info sullo stato selezionato: ClientManager.Instance.RequestTerritoryInfo("idStato");
    /// Il risultato va dentro GameManager.Instance.PuppetTerritory
    
    private int selectTerritory(TerritoryHandlerUI territory) {
        /*for (int i = 0; i < selectedTerritories.count.Length; i++) {
            Debug.Log(selectedTerritories.count[i]);
        }*/

        if (selectedTerritories.count[0] +
            selectedTerritories.count[1] + selectedTerritories.count[2] < selectedTerritories.count.Length) {
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

    //Mostra il popup per aggiungere o togliere armate
    public void SelectState(TerritoryHandlerUI newTerritory) {
        Debug.Log("State Name: " + TerritoryInformations(newTerritory.name).Name);
        stateNameAddTank.text = TerritoryInformations(newTerritory.name).Name;
        tankNumber.text = TerritoryInformations(newTerritory.name).num_tanks + "";
        popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
        popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
            popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
        popUpAddTank.SetActive(true);
    }

    /*public void SelectState(TerritoryHandlerUI newTerritory) {
        int result = selectTerritory(newTerritory);
        if (result != -1 && !newTerritory.Selected) {
            newTerritory.Select();
            tankNumber.text = TerritoryInformations(newTerritory.name).num_tanks + "";
            //tankToAdd.text = selectedTerritories.count[result] + "";
            popUpAddTank.transform.position = newTerritory.gameObject.transform.position;
            popUpAddTank.transform.position = new Vector3(popUpAddTank.transform.position.x,
                popUpAddTank.transform.position.y + (float)(0.3), popUpAddTank.transform.position.z);
            popUpAddTank.SetActive(true);
            if(selectedTerritories.count[0] +
               selectedTerritories.count[1] + selectedTerritories.count[2] == armyNumber) {
                //INVIO ARMATE DA POSIZIONARE AL SERVER
                //nella struct selectedTerritories ci sono gli stati ed il numero di armate
                armyNumber = 0;
                isTurnActive = false;
            }
        }
        /*else {
            if (selectedTerritory is not null) {
                selectedTerritory.Deselect();
            }
            selectedTerritory = newTerritory;
            selectedTerritory.Select();
        }
    }*/

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