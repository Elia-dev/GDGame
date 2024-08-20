using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAttackUI : MonoBehaviour {
    private Territory _enemyTerr;
    private Territory _myTerr;
    [SerializeField] private TMP_Text stateNameAttack;
    [SerializeField] private TMP_Text tankNumText;
    [SerializeField] private TMP_Text tankToAdd;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button attackButton;
    private int _armyNumAttck = 0;

    private void Awake() {
        plusButton.onClick.AddListener(() => AddArmy());
        minusButton.onClick.AddListener(() => RemoveArmy());
        attackButton.onClick.AddListener(() => {
            ClientManager.Instance.AttackEnemyTerritory(_myTerr, _enemyTerr, _armyNumAttck);
            this.gameObject.SetActive(false);
        });
    }

    private void AddArmy() {
        if (_armyNumAttck <= 3) {
            _armyNumAttck++;
            tankToAdd.text = _armyNumAttck + "";
        }

        if (_armyNumAttck > 1)
            attackButton.interactable = true;
        else
            attackButton.interactable = false;
    }

    private void RemoveArmy() {
        if (_armyNumAttck > 0) {
            _armyNumAttck--;
            tankToAdd.text = _armyNumAttck + "";
        }

        if (_armyNumAttck > 1)
            attackButton.interactable = true;
        else
            attackButton.interactable = false;
    }

    public void SetPupUp(Territory myTerritory, Territory enemyTerritory, GameObject gameObjTerritory) {
        _enemyTerr = enemyTerritory;
        _myTerr = myTerritory;
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

        stateNameAttack.text = enemyTerritory.name;
        this.gameObject.transform.position = gameObjTerritory.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + (float)(0.3), this.gameObject.transform.position.z);
        this.gameObject.SetActive(true);
    }
}