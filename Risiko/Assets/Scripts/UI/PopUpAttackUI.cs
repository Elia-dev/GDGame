using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAttackUI : MonoBehaviour {
    private Territory enemyTerr;
    private Territory myTerr;
    [SerializeField] private TMP_Text stateNameAttack;
    [SerializeField] private TMP_Text tankNumText;
    [SerializeField] private TMP_Text tankToAdd;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button attackButton;
    private int armyNumAttack = 0;

    private void Awake() {
        plusButton.onClick.AddListener(() => AddArmy());
        minusButton.onClick.AddListener(() => RemoveArmy());
        attackButton.onClick.AddListener(() => {
            ClientManager.Instance.AttackEnemyTerritory(myTerr, enemyTerr, armyNumAttack);
            this.gameObject.SetActive(false);
        });
    }

    private void AddArmy() {
        if (armyNumAttack < 3) {
            armyNumAttack++;
            tankToAdd.text = armyNumAttack + "";
        }

        if (armyNumAttack > 1)
            attackButton.interactable = true;
        else
            attackButton.interactable = false;
    }

    private void RemoveArmy() {
        if (armyNumAttack > 0) {
            armyNumAttack--;
            tankToAdd.text = armyNumAttack + "";
        }

        if (armyNumAttack > 1)
            attackButton.interactable = true;
        else
            attackButton.interactable = false;
    }

    public void SetPupUp(Territory myTerritory, Territory enemyTerritory, GameObject gameObjTerritory) {
        enemyTerr = enemyTerritory;
        myTerr = myTerritory;
        armyNumAttack = 0;
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
        tankToAdd.text = armyNumAttack + "";
        stateNameAttack.text = enemyTerritory.name;
        this.gameObject.transform.position = gameObjTerritory.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + (float)(0.3), this.gameObject.transform.position.z);
        this.gameObject.SetActive(true);
    }
}