using System;
using System.Collections;
using System.Collections.Generic;
using businesslogic;
using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerInfo;

    private void OnEnable() {
        /*foreach (var player in GameManager.Instance.pla) {
            playerName.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(GameManager.Instance.id), 255);
        }*/
    }
}
