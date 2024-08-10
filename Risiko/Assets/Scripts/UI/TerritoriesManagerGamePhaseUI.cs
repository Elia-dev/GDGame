using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoriesManagerGamePhaseUI : TerritoriesManagerUI
{
    public static TerritoriesManagerGamePhaseUI Instance { get; private set; }

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
                TerritoryHandlerUI territoryHandlerUI = hit.transform.GetComponent<TerritoryHandlerUI>();
                if (territoryHandlerUI is not null) {
                    SelectState(territoryHandlerUI);
                }
            } else {
                DeselectState();
            }
        }
    }

    public void SelectState(TerritoryHandlerUI newTerritory) {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
        }
        selectedTerritory = newTerritory;
        selectedTerritory.Select();
    }

    public void DeselectState() {
        if (selectedTerritory is not null) {
            selectedTerritory.Deselect();
            selectedTerritory = null;
        }
    }
}
