using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmySelectionManagerUI : MonoBehaviour
{
    public static ArmySelectionManagerUI Instance { get; private set; }
    private ArmySelectionHandlerUI selectedArmy;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // Trova il GraphicRaycaster sul Canvas
        raycaster = GetComponent<GraphicRaycaster>();

        // Trova l'EventSystem nella scena
        eventSystem = EventSystem.current;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {

            pointerEventData = new PointerEventData(eventSystem) {
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
            } else {
                DeselectArmy();
            }
        }
    }

    public void SelectArmy(ArmySelectionHandlerUI newArmy) {
        if (selectedArmy is not null) {
            selectedArmy.Deselect();
        }
        selectedArmy = newArmy;
        selectedArmy.Select();
    }

    public void DeselectArmy() {
        if (selectedArmy is not null) {
            selectedArmy.Deselect();
            selectedArmy = null;
        }
    }
}