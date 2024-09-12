using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopUpAttackUI : MonoBehaviour {
        [SerializeField] private TMP_Text stateNameAttack;
        [SerializeField] private TMP_Text tankNumText;
        [SerializeField] private TMP_Text tankToAdd;
        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private GameObject popUpAttackResult;
        
        private int _armyNumAttack = 0;
        private Territory _enemyTerr;
        private Territory _myTerr;
        
        private void Awake() {
            plusButton.onClick.AddListener(AddArmy);
            minusButton.onClick.AddListener(RemoveArmy);
            attackButton.onClick.AddListener(async () => {
                ClientManager.Instance.AttackEnemyTerritory(_myTerr, _enemyTerr, _armyNumAttack);
                this.gameObject.SetActive(false);
            });
        }

        private void AddArmy() {
            if (_armyNumAttack < 3 && _armyNumAttack < _myTerr.num_tanks-1) {
                _armyNumAttack++;
                tankToAdd.text = _armyNumAttack + "";
            }
            if (_armyNumAttack > 0)
                attackButton.interactable = true;
            else
                attackButton.interactable = false;
        }

        private void RemoveArmy() {
            if (_armyNumAttack > 0) {
                _armyNumAttack--;
                tankToAdd.text = _armyNumAttack + "";
            }
            
            if (_armyNumAttack > 0)
                attackButton.interactable = true;
            else
                attackButton.interactable = false;
        }

        public void SetPupUp(Territory myTerritory, Territory enemyTerritory, GameObject gameObjTerritory) {
            _enemyTerr = enemyTerritory;
            _myTerr = myTerritory;
            _armyNumAttack = 0;
            attackButton.interactable = false;
            string color = GameManager.Instance.GetPlayerColor(enemyTerritory.player_id);
            if (color.Equals("black") || color.Equals("blue")) {
                stateNameAttack.color = Color.white;
                tankNumText.color = Color.white;
                tankToAdd.color = Color.white;
            }
            else {
                stateNameAttack.color = Color.black;
                tankNumText.color = Color.black;
                tankToAdd.color = Color.black;
            }
            gameObject.GetComponent<Image>().color = Utils.ColorCode(color, 255);
            tankToAdd.text = _armyNumAttack + "";
            stateNameAttack.text = enemyTerritory.name;
            this.gameObject.transform.position = gameObjTerritory.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y + (float)(0.3), this.gameObject.transform.position.z);
            this.gameObject.SetActive(true);
        }
    }
}