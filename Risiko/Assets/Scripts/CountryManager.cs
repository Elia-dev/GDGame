using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public static CountryManager Instance { get; private set; }
    private StateHandler selectedState;

    private void Awake() {
        if (Instance == null) {
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

            if (hit.collider != null) {
                StateHandler stateHandler = hit.transform.GetComponent<StateHandler>();
                if (stateHandler != null) {
                    SelectState(stateHandler);
                }
            } else {
                DeselectState();
            }
        }
    }

    public void SelectState(StateHandler newState) {
        if (selectedState != null) {
            selectedState.Deselect();
        }
        selectedState = newState;
        selectedState.Select();
    }

    public void DeselectState() {
        if (selectedState != null) {
            selectedState.Deselect();
            selectedState = null;
        }
    }
}
