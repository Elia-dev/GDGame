using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class ArmySelectionManagerUI : MonoBehaviour {
        [SerializeField] private GameObject redArmy;
        [SerializeField] private GameObject greenArmy;
        [SerializeField] private GameObject blueArmy;
        [SerializeField] private GameObject yellowArmy;
        [SerializeField] private GameObject purpleArmy;
        [SerializeField] private GameObject brownArmy;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text errorMessage;
        [SerializeField] private GameObject objectiveCardCanvas;
        [SerializeField] private TMP_Text waitingLabel;
        [SerializeField] private Button chooseButton;

        private ArmySelectionHandlerUI _selectedArmy;

        private GraphicRaycaster _raycaster;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private bool _turn = false;

        private void Awake() {
            chooseButton.onClick.AddListener(() => ChooseArmy());
        }

        private void Start() {
            _raycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
        }

        private void DeactivateRaycastTargetArmy() {
            redArmy.GetComponent<Image>().raycastTarget = false;
            greenArmy.GetComponent<Image>().raycastTarget = false;
            blueArmy.GetComponent<Image>().raycastTarget = false;
            yellowArmy.GetComponent<Image>().raycastTarget = false;
            purpleArmy.GetComponent<Image>().raycastTarget = false;
            brownArmy.GetComponent<Image>().raycastTarget = false;
        }

        private void ActivateRaycastTargetArmy() {
            List<string> availableColors =
                GameManager.Instance.GetAvailableColors(); 

            foreach (var color in availableColors) {
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
                    case "brown":
                        brownArmy.GetComponent<Image>().raycastTarget = true;
                        break;
                }
            }
        }

        private void Update() {
            if (Player.Instance.IsMyTurn && !_turn) {
                //Debug.Log("Turno in army selection");
                _turn = true;
                ActivateRaycastTargetArmy();
            }

            if (Player.Instance.IsMyTurn)
                waitingLabel.gameObject.SetActive(false);
            else
                waitingLabel.gameObject.SetActive(true);

            if (Input.GetMouseButtonDown(0) && _turn) {
                _pointerEventData = new PointerEventData(_eventSystem)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                _raycaster.Raycast(_pointerEventData, results);

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
                GameObject.Find("PopUpArmySelection").SetActive(false);
                objectiveCardCanvas.SetActive(true);
            }
        }

        private void SelectArmy(ArmySelectionHandlerUI newArmy) {
            if (_selectedArmy is not null && newArmy != _selectedArmy) {
                _selectedArmy.Deselect();
            }
            _selectedArmy = newArmy;
            _selectedArmy.Select();
            errorMessage.gameObject.SetActive(false);
            title.color = Color.black;
        }

        private void ChooseArmy() {
            if (_selectedArmy is not null) {
                chooseButton.interactable = false;
                Player.Instance.ArmyColor = _selectedArmy.gameObject.name.Substring(7);
                ClientManager.Instance.SendChosenArmyColor();
                DeactivateRaycastTargetArmy();
                TerritoryHandlerUI.UserColor = Utils.ColorCode(Player.Instance.ArmyColor, 200);
                waitingLabel.gameObject.SetActive(true);
            }
            else {
                title.color = Color.red;
                errorMessage.gameObject.SetActive(true);
            }
        }
    }
}