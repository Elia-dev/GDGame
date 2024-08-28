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
    private bool _attacking;
    private bool _dataArrived = false;

    private void Awake() {
        x.onClick.AddListener(() => {
            gameObject.SetActive(false);
            TerritoriesManagerGamePhaseUI.UnderAttack = false;
        });
    }

    public async Task SetPupUp(Territory myTerritory, Territory enemyTerritory) { //, GameObject myTerritoryGObj, GameObject enemyTerritoryGObj) {
        gameObject.SetActive(true);
        //Attesa che vengano elaborati i dati dell'attacco
        //StartCoroutine(WaitUntilTrue());
        while (!GameManager.Instance.getImAttacking())
        {
            // Attende un frame prima di ricontrollare la condizione
            await Task.Yield();
        }
        _attacking = true;
        popUpAttackTitle.text = "You're attacking!";
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    public void SetPupUp() {
        gameObject.SetActive(true);
        _attacking = false;
        popUpAttackTitle.text = "You're under attack!";
        Territory enemyTerritory = GameManager.Instance.getEnemyTerritory();
        Territory myTerritory = GameManager.Instance.getMyTerritory();
        InitializeAllElement(myTerritory, enemyTerritory);
    }

    private void InitializeAllElement(Territory yoursTerritory, Territory enemyTerritory) {
        //Tu
        myInfo.text = Player.Instance.Name + "\n" +
                         "<b>" + yoursTerritory.name + "</b>" + "\nWith " + GameManager.Instance.getMyArmyNum() +" army";
        myState.sprite = loadSprite("TerritoriesSprite/" + yoursTerritory.id);
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
                    diceResult.text += myExtractedNumbers[i];
                diceResult.text += "\n";
            }
        }
        diceResult.text += "\n";

        if (GameManager.Instance.getWinnerBattleId().Equals(Player.Instance.PlayerId)) {
            Debug.Log("Winner UI " + GameManager.Instance.getEnemyNameById(GameManager.Instance.getWinnerBattleId()));
            if (_attacking)
                diceResult.text += "<color=green>You WIN!\n" + enemyTerritory.name + " now is yours!</color>";
            else
                diceResult.text += "<color=green>You WIN!\n" + enemyTerritory.name + " is safe!</color>";
        }
        else {
            Debug.Log("Winner UI " + GameManager.Instance.getEnemyNameById(GameManager.Instance.getWinnerBattleId()));
            if (_attacking)
                diceResult.text += "<color=red>You lose!</color>";
            else
                diceResult.text += "<color=red>You lose!\n" + yoursTerritory.name +
                                   " doesn't belong to you anymore!</color>";
        }
        
        //Altro giocatore
        enemyInfo.text = GameManager.Instance.getEnemyNameById(enemyTerritory.player_id)+ "\n" +
                               "<b>" + enemyTerritory.name + "</b>" + "\nWith " + GameManager.Instance.GetEnemyArmyNum() +" army";
        enemyState.sprite = loadSprite("TerritoriesSprite/" + enemyTerritory.id);
        enemyState.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(enemyTerritory.player_id), 150);
    }
    
    public Sprite loadSprite(string spriteName) {
        return Resources.Load<Sprite>(spriteName);
    }
    
    IEnumerator WaitUntilTrue()
    {
        yield return new WaitUntil(() => GameManager.Instance.getImAttacking());
        yield return new WaitUntil(() => GameManager.Instance.getMyExtractedNumbers()[0] > 0);
        Debug.Log("ImAttacking: " + GameManager.Instance.getImAttacking());
        Debug.Log("getMyExtractedNumber[0]: " + GameManager.Instance.getMyExtractedNumbers()[0]);
    }
}
