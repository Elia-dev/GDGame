using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopUpAttackResultUI : MonoBehaviour {
    [SerializeField] private Button x;
    [SerializeField] private TMP_Text popUpAttackTitle;
    [SerializeField] private TMP_Text yoursInfo;
    [SerializeField] private TMP_Text otherPlayerInfo;
    [SerializeField] private Image yourState;
    [SerializeField] private TMP_Text diceResult;
    [SerializeField] private Image otherPlayerState;
    private bool _attacking;

    private void Awake() {
        x.onClick.AddListener(() => {
            gameObject.SetActive(false);
            TerritoriesManagerGamePhaseUI.UnderAttack = false;
        });
    }

    public void SetPupUp(Territory myTerritory, Territory enemyTerritory) { //, GameObject myTerritoryGObj, GameObject enemyTerritoryGObj) {
        gameObject.SetActive(true);
        _attacking = false;
        popUpAttackTitle.text = "You're attacking!";
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    public void SetPupUp() {
        gameObject.SetActive(true);
        _attacking = true;
        popUpAttackTitle.text = "You're under attack!";
        Territory enemyTerritory = GameManager.Instance.getEnemyTerritory();
        Territory myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    private void InitializeAllElement(Territory yoursTerritory, Territory enemyTerritory) {
        //Tu
        yoursInfo.text = Player.Instance.Name + "\n" +
                         "<b>" + yoursTerritory.name + "</b>" + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        yourState.sprite = loadSprite("TerritoriesSprite/" + yoursTerritory.id);
        yourState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);

        int[] myExtractedNumbers = GameManager.Instance.getMyExtractedNumbers();
        int[] enemyExtractedNumbers = GameManager.Instance.getEnemyExtractedNumbers();
        Debug.Log("MyExtractedNumber UI: " + myExtractedNumbers);
        Debug.Log("EnemyExtractedNumber UI: " + enemyExtractedNumbers);
        diceResult.text = "<b>Dice results</b>\n";
        if (myExtractedNumbers.Length <= enemyExtractedNumbers.Length) {
            for (int i = 0; i < enemyExtractedNumbers.Length; i++) {
                if (myExtractedNumbers.Length > i)
                    diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                else
                    diceResult.text += "     " + enemyExtractedNumbers[i];
                diceResult.text += "\n";
            }
        } else {
            for (int i = 0; i < myExtractedNumbers.Length; i++) {
                if (enemyExtractedNumbers.Length > i)
                    diceResult.text += myExtractedNumbers[i] + " - " + enemyExtractedNumbers[i];
                else
                    diceResult.text += myExtractedNumbers[i];
                diceResult.text += "\n";
            }
        }

        if (GameManager.Instance.getWinnerBattleId().Equals(Player.Instance.PlayerId))
            if(_attacking)
                diceResult.text += "<color=green>You WIN!\n" + enemyTerritory.name + " now is yours!</color>";
            else 
                diceResult.text += "<color=green>You WIN!\n" + enemyTerritory.name + " is safe!</color>";
        else
            if(_attacking)
                diceResult.text += "<color=red>You lose!</color>";
            else
                diceResult.text += "<color=red>You lose!\n" + yoursTerritory.name + " doesn't belong to you anymore!</color>";
        
        //Altro giocatore
        otherPlayerInfo.text = GameManager.Instance.getEnemyNameById(enemyTerritory.player_id)+ "\n" +
                               "<b>" + enemyTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        otherPlayerState.sprite = loadSprite("TerritoriesSprite/" + enemyTerritory.id);
        otherPlayerState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(enemyTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
}
