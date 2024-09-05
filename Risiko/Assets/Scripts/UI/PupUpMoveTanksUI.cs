using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PupUpMoveTanksUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text stateNameMove;
        [SerializeField] private TMP_Text tankNumText;
        [SerializeField] private TMP_Text tankToAdd;
        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button moveButton;
        
        private Territory _fromTerritory;
        private Territory _toTerritory;
        private int _armyToMove = 0;
    
        private void Awake() {
            plusButton.onClick.AddListener(() => AddArmy());
            minusButton.onClick.AddListener(() => RemoveArmy());
            moveButton.onClick.AddListener(() => {
                _fromTerritory.num_tanks -= _armyToMove;
                _toTerritory.num_tanks += _armyToMove;
                TerritoriesManagerGamePhaseUI.StategicMove = true;
                ClientManager.Instance.UpdateTerritoriesState();
                this.gameObject.SetActive(false);
            });
        }

        private void AddArmy() {
            // Se il numero di truppe da spostare è minore del numero di truppe presenti
            // nel territorio incremento il numero di truppe da spostare
            if(_armyToMove < _fromTerritory.num_tanks-1){
                _armyToMove++;
                tankToAdd.text = _armyToMove + "";
            }

            if (_armyToMove > 0)
                moveButton.interactable = true;
            else
                moveButton.interactable = false;
        }
    
        private void RemoveArmy() {
            // Se il numero di truppe da spostare è maggiore di 0 decremento il numero di truppe da spostare
            if (_armyToMove > 0) {
                _armyToMove--;
                tankToAdd.text = _armyToMove + "";
            }

            if (_armyToMove > 0)
                moveButton.interactable = true;
            else
                moveButton.interactable = false;
        }

        public void SetPupUp(Territory fromTerritory, Territory toTerritory, GameObject gameObjTerritory) {
            this._fromTerritory = fromTerritory;
            this._toTerritory = toTerritory;
            _armyToMove = 0;
            moveButton.interactable = false;
            string color = Player.Instance.ArmyColor;
            // Cambio colore del testo in base al colore dell'armata per migliorare la leggibilità
            if (color.Equals("black") || color.Equals("blue")) {
                stateNameMove.color = Color.white;
                tankNumText.color = Color.white;
                tankToAdd.color = Color.white;
            }
            else {
                stateNameMove.color = Color.black;
                tankNumText.color = Color.black;
                tankToAdd.color = Color.black;
            }
            // Cambio colore del bottone in base al colore dell'armata
            gameObject.GetComponent<Image>().color = Utils.ColorCode(color, 255);
            tankToAdd.text = _armyToMove + "";
            stateNameMove.text = toTerritory.name;
            this.gameObject.transform.position = gameObjTerritory.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y + (float)(0.3), this.gameObject.transform.position.z);
            this.gameObject.SetActive(true);
        }
    }
}
