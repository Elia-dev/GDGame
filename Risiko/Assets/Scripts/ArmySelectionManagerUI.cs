using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArmySelectionManagerUI : MonoBehaviour {
    private static ArmySelectionManagerUI Instance { get; set; }
    private ArmySelectionHandlerUI selectedArmy;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    public int playerNumber = 4;
    [SerializeField] private GameObject redArmy;
    [SerializeField] private GameObject greenArmy;
    [SerializeField] private GameObject blueArmy;
    [SerializeField] private GameObject yellowArmy;
    [SerializeField] private GameObject purpleArmy;
    [SerializeField] private GameObject blackArmy;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text errorMessage;

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
        //VA IMPOSTATO IL NUMERO DI GIOCATORI
        switch (playerNumber) {
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
        
        //SCORRERE LA LISTA DELLE ARMATE GIà PRESE DAGLI ALTRI GIOCATORI E DISATTIVARE I raycastTarget ALLE CORRISPONDENTI ARMATE
        //greenArmy.GetComponent<Image>().raycastTarget = false;
        
        // Trova il GraphicRaycaster sul Canvas
        raycaster = GetComponent<GraphicRaycaster>();

        // Trova l'EventSystem nella scena
        eventSystem = EventSystem.current;
    }
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
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
            //COMUNICA AL SERVER L'ARMATA
            //ATTENDE COMUNICAZIONE DAL SERVER PER PASSARE ALLA PROSSIMA FASE
            //LANCIA LA PROSSIMA FASE
            Color32 color = selectedArmy.ArmyColor;
            color.a = 100;
            CountryHandlerUI.userColor = color;
            CountryHandlerUI.ArmyDistributionPhase();
            GameObject.Find("PopUpArmySelection").SetActive(false);
        }
        else {
            title.color = Color.red;
            errorMessage.gameObject.SetActive(true);
        }
    }
}