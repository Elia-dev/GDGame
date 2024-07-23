using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySelectionManagerUI : MonoBehaviour
{
    private StateHandlerUI selectedState;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider is not null) {
                StateHandlerUI stateHandlerUI = hit.transform.GetComponent<StateHandlerUI>();
                if (stateHandlerUI is not null) {
                    SelectState(stateHandlerUI);
                }
            } else {
                DeselectState();
            }
        }
    }

    public void SelectState(StateHandlerUI newState) {
        if (selectedState is not null) {
            selectedState.Deselect();
        }
        selectedState = newState;
        selectedState.Select();
    }

    public void DeselectState() {
        if (selectedState is not null) {
            selectedState.Deselect();
            selectedState = null;
        }
    }
}
