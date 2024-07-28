using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryManagerDistrPhaseUI : MonoBehaviour {
    //DA FINIRE, AVEVO SALTATO UNA FASE OPS
    public static CountryManagerDistrPhaseUI Instance { get; private set; }
    private CountryHandlerUI selectedCountry;

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
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider is not null) {
                CountryHandlerUI countryHandlerUI = hit.transform.GetComponent<CountryHandlerUI>();
                if (countryHandlerUI is not null) {
                    Debug.Log(countryHandlerUI.Selected);
                    if (countryHandlerUI.Selected) {
                        DeselectState(countryHandlerUI);
                    }
                    else {
                        SelectState(countryHandlerUI);
                    }
                }
            }
        }
    }

    public void SelectState(CountryHandlerUI newCountry) {
        newCountry.Select();
    }

    public void DeselectState(CountryHandlerUI country) {
        country.Deselect();
    }
}