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

    private void Awake() {
        x.onClick.AddListener(() => {
            gameObject.SetActive(false);
            TerritoriesManagerGamePhaseUI.UnderAttack = false;
        });
    }

    public void SetPupUp(Territory myTerritory, Territory enemyTerritory) { //, GameObject myTerritoryGObj, GameObject enemyTerritoryGObj) {
        gameObject.SetActive(true);
        popUpAttackTitle.text = "You're attacking!";
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    public void SetPupUp() {
        gameObject.SetActive(true);
        popUpAttackTitle.text = "You're under attack!";
        Territory enemyTerritory = GameManager.Instance.getEnemyTerritory();
        Territory myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    private void InitializeAllElement(Territory yoursTerritory, Territory OtherPlayerTerritory) {
        //Tu
        yoursInfo.text = Player.Instance.Name + "\n" +
                         "<b>" + yoursTerritory.name + "</b>" + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        yourState.sprite = loadSprite("TerritoriesSprite/" + yoursTerritory.id);
        yourState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);
        
        diceResult.text = "Soon available";
        
        //Altro giocatore
        otherPlayerInfo.text = GameManager.Instance.getEnemyNameById(OtherPlayerTerritory.player_id)+ "\n" +
                               "<b>" + OtherPlayerTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        otherPlayerState.sprite = loadSprite("TerritoriesSprite/" + OtherPlayerTerritory.id);
        otherPlayerState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(OtherPlayerTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
}
