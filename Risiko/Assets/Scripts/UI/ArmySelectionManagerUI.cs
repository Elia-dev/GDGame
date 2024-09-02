using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ArmySelectionManagerUI : MonoBehaviour {
        private ArmySelectionHandlerUI _selectedArmy;

        private GraphicRaycaster _raycaster;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private bool _turn = false;

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
            chooseButton.onClick.AddListener( () => ChooseArmy());
        }

        private void Start() {
            // Trova il GraphicRaycaster sul Canvas
            _raycaster = GetComponent<GraphicRaycaster>();

            // Trova l'EventSystem nella scena
            _eventSystem = EventSystem.current;
        }
        //Disattivazione di tutti i raycast
        private void DeactivateRaycastTargetArmy() {
            redArmy.GetComponent<Image>().raycastTarget = false;
            greenArmy.GetComponent<Image>().raycastTarget = false;
            blueArmy.GetComponent<Image>().raycastTarget = false;
            yellowArmy.GetComponent<Image>().raycastTarget = false;
            purpleArmy.GetComponent<Image>().raycastTarget = false;
            blackArmy.GetComponent<Image>().raycastTarget = false;
        }
        //Metodo che attiva solo i reaycast delle armate disponibili
        private void ActivateRaycastTargetArmy() {
            List<string>
                AvailableColors = GameManager.Instance.GetAvailableColors(); // Per prendere la lista dei colori disponibili

            foreach (var color in AvailableColors) {
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
            if (Player.Instance.IsMyTurn && !_turn) {
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
            //Lancio fase successiva quando vengono rivenute le carte obiettivo
            if (Player.Instance.ObjectiveCard is not null) {
                //TerritoryHandlerUI.ArmyDistributionPhase();
                GameObject.Find("PopUpArmySelection").SetActive(false);
                //Popup carte obiettivo
                objectiveCardCanvas.SetActive(true);
            }
        }

        public void SelectArmy(ArmySelectionHandlerUI newArmy) {
            if (_selectedArmy is not null && newArmy != _selectedArmy) {
                _selectedArmy.Deselect();
            }

            _selectedArmy = newArmy;
            _selectedArmy.Select();
            errorMessage.gameObject.SetActive(false);
            title.color = Color.black;
        }

        public void ChooseArmy() {
            if (_selectedArmy is not null) {
                //Tasto non più interagibile
                chooseButton.interactable = false;
                Player.Instance.ArmyColor = _selectedArmy.gameObject.name.Substring(7);
                //COMUNICA AL SERVER L'ARMATA
                ClientManager.Instance.SendChosenArmyColor();
                //Disattivo i raycast dei carriarmati
                DeactivateRaycastTargetArmy();

                //Preparazione prossima fase
                //Color32 color = selectedArmy.ArmyColor;
                //color.a = 200;
                //TerritoryHandlerUI.userColor = color;
                TerritoryHandlerUI.UserColor = Utils.ColorCode(Player.Instance.ArmyColor, 200);
                waitingLabel.gameObject.SetActive(true);
                //gameObject.GetComponent<Renderer>().enabled = false;
            }
            else {//Se non è stata selezionata un'armata mostra errore
                title.color = Color.red;
                errorMessage.gameObject.SetActive(true);
            }
        }
    }
}