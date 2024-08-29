using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
            GameManager.Instance.cleanAfterBattle();
            TerritoriesManagerGamePhaseUI.UnderAttack = false;
        });
    }

    private void Update() {
        if (!GameManager.Instance.getWinnerBattleId().Equals("")  && !_dataArrived) {
            _dataArrived = true;
            if (GameManager.Instance.getWinnerBattleId().Equals(Player.Instance.PlayerId)) {
                diceResult.text += "<color=green>You WIN!\n" + _enemyTerritory.name + " now is yours!</color>";
            }
            else {
                diceResult.text += "<color=red>You lose!\n" + _myTerritory.name +
                                   " doesn't belong to you anymore!</color>";
            }
        }
    }

    /*public async Task SetPupUp(Territory myTerritory, Territory enemyTerritory) { //, GameObject myTerritoryGObj, GameObject enemyTerritoryGObj) {
        _dataArrived = false;
        gameObject.SetActive(true);
        //Attesa che vengano elaborati i dati dell'attacco
        //StartCoroutine(WaitUntilTrue());
        while (!GameManager.Instance.getImAttacking())
        {
            // Attende un frame prima di ricontrollare la condizione
            await Task.Delay(100);
        }
        Debug.Log("ImAttacking: " + GameManager.Instance.getImAttacking());
        Debug.Log("getMyExtractedNumber[0]: " + GameManager.Instance.getMyExtractedNumbers()[0]); //NullReferenceException
        this.myTerritory = myTerritory;
        this.enemyTerritory = enemyTerritory;
        _attacking = true;
        popUpAttackTitle.text = "You're attacking!";
        InitializeAllElement(myTerritory, enemyTerritory);
    }*/

    /*public void SetPupUp() {
        _dataArrived = false;
        gameObject.SetActive(true);
        //_attacking = false;
        if(GameManager.Instance.getImAttacking())
            popUpAttackTitle.text = "You're attacking!";
        else
            popUpAttackTitle.text = "You're under attack!";
        this.enemyTerritory = GameManager.Instance.getEnemyTerritory();
        this.myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }*/

    public void SetPupUp() {
        _dataArrived = false;
        gameObject.SetActive(true);
        if(GameManager.Instance.getImAttacking())
            popUpAttackTitle.text = "You're attacking!";
        else
            popUpAttackTitle.text = "You're under attack!";
        this._enemyTerritory = GameManager.Instance.getEnemyTerritory();
        this._myTerritory = GameManager.Instance.getMyTerritory();
        //Tu
        myInfo.text = Player.Instance.Name + "\n" +
                         "<b>" + _myTerritory.name + "</b>" + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        myState.sprite = loadSprite("TerritoriesSprite/" + _myTerritory.id);
        myState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);

        int[] myExtractedNumbers = GameManager.Instance.getMyExtractedNumbers();
        int[] enemyExtractedNumbers = GameManager.Instance.getEnemyExtractedNumbers();
        diceResult.text = "\n<b>Dice results</b>\n";
        if (myExtractedNumbers.Length <= enemyExtractedNumbers.Length) {
            for (int i = 0; i < enemyExtractedNumbers.Length; i++) {
                if (myExtractedNumbers.Length > i)
                    diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                else
                    diceResult.text += "        " + enemyExtractedNumbers[i];
                diceResult.text += "\n";
            }
        } else {
            for (int i = 0; i < myExtractedNumbers.Length; i++) {
                if (enemyExtractedNumbers.Length > i)
                    diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                else
                    diceResult.text += myExtractedNumbers[i] + "<color=#FFFFFF00> - 0</color>";
                diceResult.text += "\n";
            }
        }
        diceResult.text += "\n";
        
        //Altro giocatore
        enemyInfo.text = GameManager.Instance.getEnemyNameById(_enemyTerritory.player_id)+ "\n" +
                               "<b>" + _enemyTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        enemyState.sprite = loadSprite("TerritoriesSprite/" + _enemyTerritory.id);
        enemyState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(_enemyTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
}
