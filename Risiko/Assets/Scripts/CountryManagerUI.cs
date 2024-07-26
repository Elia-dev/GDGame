using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryManagerUI : MonoBehaviour
{
    public static CountryManagerUI Instance { get; private set; }
    private CountryHandlerUI selectedCountry;

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
                CountryHandlerUI countryHandlerUI = hit.transform.GetComponent<CountryHandlerUI>();
                if (countryHandlerUI is not null) {
                    SelectState(countryHandlerUI);
                }
            } else {
                DeselectState();
            }
        }
    }

    public void SelectState(CountryHandlerUI newCountry) {
        if (selectedCountry is not null) {
            selectedCountry.Deselect();
        }
        selectedCountry = newCountry;
        selectedCountry.Select();
    }

    public void DeselectState() {
        if (selectedCountry is not null) {
            selectedCountry.Deselect();
            selectedCountry = null;
        }
    }
}
