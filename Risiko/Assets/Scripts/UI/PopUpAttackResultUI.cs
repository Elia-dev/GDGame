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
        Territory enemyTerr = GameManager.Instance.getEnemyTerritory();
        Territory myTerr = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    public void SetPupUp() {
        gameObject.SetActive(true);
        popUpAttackTitle.text = "You're under attack!";
        Territory enemyTerritory = GameManager.Instance.getEnemyTerritory();
        Territory myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    private void InitializeAllElement(Territory yoursTerritory, Territory OtherPLayerTerritory) {
        //Tu
        //Territory enemyTerr = GameManager.Instance.getEnemyTerritory();
        yoursInfo.text = Player.Instance.Name + "\n" +
                         yoursTerritory.name + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        yourState.sprite = loadSprite("TerrritoriesSprite/" + yoursTerritory.id);
        yourState.color = Utils.ColorCode(Player.Instance.ArmyColor, 150);
        
        diceResult.text = "Soon available";
        
        //Altro giocatore
        //Territory myTerr = GameManager.Instance.getMyTerritory();
        otherPlayerInfo.text = GameManager.Instance.getEnemyNameById(OtherPLayerTerritory.player_id)+ "\n" +
                               OtherPLayerTerritory.name + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        otherPlayerState.sprite = loadSprite("TerrritoriesSprite/" + OtherPLayerTerritory.id);
        otherPlayerState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(yoursTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
}
