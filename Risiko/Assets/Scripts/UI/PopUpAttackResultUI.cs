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
    private bool _attacking;
    private bool _dataArrived = false;
    private Territory enemyTerritory;
    private Territory myTerritory;

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
                Debug.Log("Winner UI " + GameManager.Instance.getEnemyNameById(GameManager.Instance.getWinnerBattleId()));
                diceResult.text += "<color=green>You WIN!\n" + enemyTerritory.name + " now is yours!</color>";
            }
            else {
                Debug.Log("Winner UI " + GameManager.Instance.getEnemyNameById(GameManager.Instance.getWinnerBattleId()));
                diceResult.text += "<color=red>You lose!\n" + myTerritory.name +
                                   " doesn't belong to you anymore!</color>";
            }
        }
    }

    public async Task SetPupUp(Territory myTerritory, Territory enemyTerritory) { //, GameObject myTerritoryGObj, GameObject enemyTerritoryGObj) {
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
    }

    public void SetPupUp() {
        _dataArrived = false;
        gameObject.SetActive(true);
        _attacking = false;
        popUpAttackTitle.text = "You're under attack!";
        this.enemyTerritory = GameManager.Instance.getEnemyTerritory();
        this.myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    private void InitializeAllElement(Territory myTerritory, Territory enemyTerritory) {
        //Tu
        myInfo.text = Player.Instance.Name + "\n" +
                         "<b>" + myTerritory.name + "</b>" + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        myState.sprite = loadSprite("TerritoriesSprite/" + myTerritory.id);
        myState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);

        int[] myExtractedNumbers = GameManager.Instance.getMyExtractedNumbers();
        int[] enemyExtractedNumbers = GameManager.Instance.getEnemyExtractedNumbers();
        Debug.Log("MyExtractedNumber UI: ");
        foreach (var myNum in myExtractedNumbers) {
            Debug.Log(myNum);
        }
        Debug.Log("EnemyExtractedNumber UI: ");
        foreach (var enemyNum in myExtractedNumbers) {
            Debug.Log(enemyNum);
        }
        diceResult.text = "<b>Dice results</b>\n";
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
                    diceResult.text += myExtractedNumbers[i] + "        \n";
                diceResult.text += "\n";
            }
        }
        diceResult.text += "\n";
        
        //Altro giocatore
        enemyInfo.text = GameManager.Instance.getEnemyNameById(enemyTerritory.player_id)+ "\n" +
                               "<b>" + enemyTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        enemyState.sprite = loadSprite("TerritoriesSprite/" + enemyTerritory.id);
        enemyState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(enemyTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
}
