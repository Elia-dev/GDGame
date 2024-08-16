using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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
    [SerializeField] private Button chooseButton;

    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        
        chooseButton.onClick.AddListener( () => ChooseArmy());
    }

    private void Start() {
        /*List<string>
            AvailableColors = GameManager.Instance.GetAvailableColors(); // Per prendere la lista dei colori disponibili

        foreach (var color in AvailableColors) {
            switch (color) {
                case "red":
                    redArmy.GameObject().SetActive(true);
                    break;
                case "green":
                    greenArmy.GameObject().SetActive(true);
                    break;
                case "blue":
                    blueArmy.GameObject().SetActive(true);
                    break;
                case "yellow":
                    yellowArmy.GameObject().SetActive(true);
                    break;
                case "purple":
                    purpleArmy.GameObject().SetActive(true);
                    break;
                case "black":
                    blackArmy.GameObject().SetActive(true);
                    break;
            }
        }*/
        /*switch (GameManager.Instance.PlayersName.Count) {
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
        }*/

        // Trova il GraphicRaycaster sul Canvas
        raycaster = GetComponent<GraphicRaycaster>();

        // Trova l'EventSystem nella scena
        eventSystem = EventSystem.current;
    }

    private void DeactivateRaycastTargetArmy() {
        redArmy.GetComponent<Image>().raycastTarget = false;
        greenArmy.GetComponent<Image>().raycastTarget = false;
        blueArmy.GetComponent<Image>().raycastTarget = false;
        yellowArmy.GetComponent<Image>().raycastTarget = false;
        purpleArmy.GetComponent<Image>().raycastTarget = false;
        blackArmy.GetComponent<Image>().raycastTarget = false;
    }
    
    private void ActivateRaycastTargetArmy() {
        List<string>
            AvailableColors = GameManager.Instance.GetAvailableColors(); // Per prendere la lista dei colori disponibili

        foreach (var color in AvailableColors) {
            Debug.Log("UI color: " + color);
            switch (color) {
                case "red":
                    redArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "green":
                    greenArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "blue":
                    blueArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "yellow":
                    yellowArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "purple":
                    purpleArmy.GetComponent<Image>().raycastTarget = true;
                    break;
                case "black":
                    blackArmy.GetComponent<Image>().raycastTarget = true;
                    break;
            }
        }
    }

    private void Update() {
        //Start del turno
        if (Player.Instance.IsMyTurn && !turn) {
            turn = true;
            ActivateRaycastTargetArmy();
        }

        if (Player.Instance.IsMyTurn)
            waitingLabel.gameObject.SetActive(false);
        else 
            waitingLabel.gameObject.SetActive(true);
        
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
        //Lancio fase successiva quando vengono rivenute le carte obiettivo
        if (Player.Instance.ObjectiveCard is not null) {
            TerritoryHandlerUI.ArmyDistributionPhase();
            GameObject.Find("PopUpArmySelection").SetActive(false);
            //Popup carte obiettivo
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
            chooseButton.interactable = false;
            Player.Instance.ArmyColor = selectedArmy.gameObject.name.Substring(7);
            //COMUNICA AL SERVER L'ARMATA
            ClientManager.Instance.SendChosenArmyColor();
            //Disattivo i raycast dei carriarmati
            DeactivateRaycastTargetArmy();
            //turn = false;

            //Preparazione prossima fase
            Color32 color = selectedArmy.ArmyColor;
            color.a = 200;
            TerritoryHandlerUI.userColor = color;
            waitingLabel.gameObject.SetActive(true);
            //gameObject.GetComponent<Renderer>().enabled = false;
        }
        else {
            title.color = Color.red;
            errorMessage.gameObject.SetActive(true);
        }
    }
}