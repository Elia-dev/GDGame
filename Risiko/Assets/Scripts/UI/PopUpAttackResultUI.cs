using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class PopUpAttackResultUI : MonoBehaviour {
        [SerializeField] private Button x;
        [SerializeField] private TMP_Text popUpAttackTitle;
        [SerializeField] private TMP_Text myInfo;
        [SerializeField] private TMP_Text enemyInfo;
        [SerializeField] private Image myState;
        [SerializeField] private TMP_Text diceResult;
        [SerializeField] private Image enemyState;
        [SerializeField] private GameObject popUpContainer;
        [SerializeField] private GameObject popUpContainerResult;
        [SerializeField] private TMP_Text attackResult;
        
        private bool _dataArrived = false;
        private Territory _enemyTerritory;
        private Territory _myTerritory;

        private void Awake() {
            x.onClick.AddListener(() => {
                gameObject.SetActive(false);
                GameManager.Instance.CleanAfterBattle();
                TerritoriesManagerGamePhaseUI.UnderAttack = false;
            });
        }

        private void Update() {
            if (!GameManager.Instance.GetWinnerBattleId().Equals("") && !_dataArrived) {
                _dataArrived = true;
                if (GameManager.Instance.GetWinnerBattleId().Equals(Player.Instance.PlayerId)) {
                    diceResult.text += "<color=green>You WIN!\n" + _enemyTerritory.name + " now is yours!</color>";
                }
                else {
                    diceResult.text += "<color=red>You lose!\n" + _myTerritory.name +
                                       " doesn't belong to you anymore!</color>";
                }
            }
        }

        public void SetPupUp() {
            _dataArrived = false;
            gameObject.SetActive(true);
            if (GameManager.Instance.GetImAttacking())
                popUpAttackTitle.text = "You're attacking!";
            else
                popUpAttackTitle.text = "You're under attack!";
            this._enemyTerritory = GameManager.Instance.GetEnemyTerritory();
            this._myTerritory = GameManager.Instance.GetMyTerritory();
            myInfo.text = Player.Instance.Name + "\n" +
                          "<b>" + _myTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetMyArmyNum() +
                          " army";
            myState.sprite = LoadSprite("TerritoriesSprite/" + _myTerritory.id);
            myState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);
            
            int[] myExtractedNumbers = GameManager.Instance.GetMyExtractedNumbers();
            int[] enemyExtractedNumbers = GameManager.Instance.GetEnemyExtractedNumbers();
            diceResult.text = "\n<b>Dice results</b>\n";
            if (myExtractedNumbers.Length <= enemyExtractedNumbers.Length) {
                for (int i = 0; i < enemyExtractedNumbers.Length; i++) {
                    if (myExtractedNumbers.Length > i)
                        diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                    else
                        diceResult.text += "        " + enemyExtractedNumbers[i];
                    diceResult.text += "\n";
                }
                switch (enemyExtractedNumbers.Length) {
                    case 1:
                        diceResult.text += "\n\n";
                        break;
                    case 2:
                        diceResult.text += "\n";
                        break;
                }
            }
            else {
                for (int i = 0; i < myExtractedNumbers.Length; i++) {
                    if (enemyExtractedNumbers.Length > i)
                        diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                    else
                        diceResult.text += myExtractedNumbers[i] + "<color=#FFFFFF00> - 0</color>";
                    diceResult.text += "\n";
                }
                switch (myExtractedNumbers.Length) {
                    case 1:
                        diceResult.text += "\n\n";
                        break;
                    case 2:
                        diceResult.text += "\n";
                        break;
                }
            }
            diceResult.text += "\n";

            enemyInfo.text = GameManager.Instance.GetEnemyNameById(_enemyTerritory.player_id) + "\n" +
                             "<b>" + _enemyTerritory.name + "</b>" + "\nWith " +
                             GameManager.Instance.GetEnemyArmyNum() + " army";
            enemyState.sprite = LoadSprite("TerritoriesSprite/" + _enemyTerritory.id);
            enemyState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(_enemyTerritory.player_id), 150);
        }

        private Sprite LoadSprite(string spriteName) {
            return Resources.Load<Sprite>(spriteName);
        }
    }
}