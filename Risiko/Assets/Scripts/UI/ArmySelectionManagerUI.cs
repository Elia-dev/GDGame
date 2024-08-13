using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ArmySelectionManagerUI : MonoBehaviour {
    private static ArmySelectionManagerUI Instance { get; set; }
    private ArmySelectionHandlerUI selectedArmy;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private bool turn = false;

    [SerializeField] private GameObject redArmy;
    [SerializeField] private GameObject greenArmy;
    [SerializeField] private GameObject blueArmy;
    [SerializeField] private GameObject yellowArmy;
    [SerializeField] private GameObject purpleArmy;
    [SerializeField] private GameObject blackArmy;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private GameObject objectiveCardCanvas;
    [SerializeField] private TMP_Text waitingLabel;

    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        switch (GameManager.Instance.PlayersName.Count) {
            case 4:
                yellowArmy.GameObject().SetActive(true);
                break;
            case 5:
                yellowArmy.GameObject().SetActive(true);
                purpleArmy.GameObject().SetActive(true);
                break;
            case 6:
                yellowArmy.GameObject().SetActive(true);
                purpleArmy.GameObject().SetActive(true);
                blackArmy.GameObject().SetActive(true);
                break;
        }

        // Trova il GraphicRaycaster sul Canvas
        raycaster = GetComponent<GraphicRaycaster>();

        // Trova l'EventSystem nella scena
        eventSystem = EventSystem.current;
    }

    private void ActivateRaycastTargetArmy() {
        List<string>
            AvailableColors = GameManager.Instance.GetAvailableColors(); // Per prendere la lista dei colori disponibili
        foreach (var color in AvailableColors) {
            Debug.Log(color);
        }

        foreach (var color in AvailableColors) {
            switch (color) {
                case "red,":
                    redArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "green,":
                    greenArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "blue,":
                    blueArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "yellow,":
                    yellowArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "purple,":
                    purpleArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "black":
                    blackArmy.GetComponent<Image>().raycastTarget = true;
                    break;
            }
        }
    }

    private void Update() {
        Debug.Log("IsMyTurn: " + Player.Instance.IsMyTurn);
        if (Player.Instance.IsMyTurn && !turn) {
            turn = true;
            ActivateRaycastTargetArmy();
        }

        if (Input.GetMouseButtonDown(0) && turn) {
            pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0) {
                foreach (RaycastResult result in results) {
                    ArmySelectionHandlerUI armyHandlerUI = result.gameObject.GetComponent<ArmySelectionHandlerUI>();
                    if (armyHandlerUI is not null) {
                        SelectArmy(armyHandlerUI);
                        return;
                    }
                }
            }
        }

        if (Player.Instance.ObjectiveCard is not null) {
            TerritoryHandlerUI.ArmyDistributionPhase();
            GameObject.Find("PopUpArmySelection").SetActive(false);
            //RICEZIONE OGGETTO CARTA DA PARTE DEL SERVER
            // La carta objective è memorizzata qui Player.Instance.ObjectiveCard
            objectiveCardCanvas.SetActive(true);
        }
    }

    public void SelectArmy(ArmySelectionHandlerUI newArmy) {
        if (selectedArmy is not null && newArmy != selectedArmy) {
            selectedArmy.Deselect();
        }

        selectedArmy = newArmy;
        selectedArmy.Select();
        errorMessage.gameObject.SetActive(false);
        title.color = Color.black;
    }

    public void ChooseArmy() {
        if (selectedArmy is not null) {
            Player.Instance.ArmyColor = selectedArmy.gameObject.name.Substring(6);
            Debug.Log(selectedArmy.gameObject.name.Substring(6));
            ClientManager.Instance.SendChosenArmyColor();
            turn = false;
            //COMUNICA AL SERVER L'ARMATA

            // Per mandare l'armata settare prima il colore in string usando player.ArmyColor="colore scelto"
            // Poi utilizzare questo comando ClientManager.Instance.SendChosenArmyColor();
            //ATTENDE COMUNICAZIONE DAL SERVER PER PASSARE ALLA PROSSIMA FASE
            // Per vedere se è il tuo turno puoi usare Player.Instance.IsMyTurn();

            //LANCIA LA PROSSIMA FASE
            Color32 color = selectedArmy.ArmyColor;
            color.a = 100;
            TerritoryHandlerUI.userColor = color;
            waitingLabel.gameObject.SetActive(true);
            gameObject.GetComponent<Renderer>().enabled = false;
                
            /*//TerritoryHandlerUI.ArmyDistributionPhase();
            GameObject.Find("PopUpArmySelection").SetActive(false);
            //RICEZIONE OGGETTO CARTA DA PARTE DEL SERVER
            // La carta objective è memorizzata qui Player.Instance.ObjectiveCard
            objectiveCardCanvas.SetActive(true);*/
        }
        else {
            title.color = Color.red;
            errorMessage.gameObject.SetActive(true);
        }
    }
}