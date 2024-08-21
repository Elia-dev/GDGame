using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PupUpMoveTanksUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stateNameMove;
    [SerializeField] private TMP_Text tankNumText;
    [SerializeField] private TMP_Text tankToAdd;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button moveButton;
    private Territory fromTerritory;
    private Territory toTerritory;
    private int armyToMove = 0;
    
    private void Awake() {
        plusButton.onClick.AddListener(() => AddArmy());
        minusButton.onClick.AddListener(() => RemoveArmy());
        moveButton.onClick.AddListener(() => {
            fromTerritory.num_tanks -= armyToMove;
            toTerritory.num_tanks += armyToMove;
            ClientManager.Instance.UpdateTerritoriesState();
            this.gameObject.SetActive(false);
        });
    }

    private void AddArmy() {
        if(armyToMove < toTerritory.num_tanks-1){
            armyToMove++;
            tankToAdd.text = armyToMove + "";
        }

        if (armyToMove > 0)
            moveButton.interactable = true;
        else
            moveButton.interactable = false;
    }
    
    private void RemoveArmy() {
        if (armyToMove > 0) {
            armyToMove--;
            tankToAdd.text = armyToMove + "";
        }

        if (armyToMove > 1)
            moveButton.interactable = true;
        else
            moveButton.interactable = false;
    }

    public void SetPupUp(Territory fromTerritory, Territory toTerritory, GameObject gameObjTerritory) {
        this.fromTerritory = fromTerritory;
        this.toTerritory = toTerritory;
        armyToMove = 0;
        string color = Player.Instance.ArmyColor;
        if (color.Equals("black") || color.Equals("blue")) {
            stateNameMove.color = Color.white;
            tankNumText.color = Color.white;
            tankToAdd.color = Color.white;
        }
        else {
            stateNameMove.color = Color.black;
            tankNumText.color = Color.black;
            tankToAdd.color = Color.black;
        }
        gameObject.GetComponent<Image>().color = Utils.ColorCode(color, 255);
        tankToAdd.text = armyToMove + "";
        stateNameMove.text = toTerritory.name;
        this.gameObject.transform.position = gameObjTerritory.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + (float)(0.3), this.gameObject.transform.position.z);
        this.gameObject.SetActive(true);
    }
}
