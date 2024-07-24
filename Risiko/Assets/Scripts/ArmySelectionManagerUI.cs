using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySelectionManagerUI : MonoBehaviour
{
    public static ArmySelectionManagerUI Instance { get; private set; }
    private ArmySelectionHandlerUI selectedState;

    private void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider is not null) {
                ArmySelectionHandlerUI armyHandlerUI = hit.transform.GetComponent<ArmySelectionHandlerUI>();
                if (armyHandlerUI is not null) {
                    SelectArmy(armyHandlerUI);
                }
            } else {
                DeselectArmy();
            }
        }
    }

    public void SelectArmy(ArmySelectionHandlerUI newArmy) {
        if (selectedState is not null) {
            selectedState.Deselect();
        }
        selectedState = newArmy;
        selectedState.Select();
    }

    public void DeselectArmy() {
        if (selectedState is not null) {
            selectedState.Deselect();
            selectedState = null;
        }
    }
}
