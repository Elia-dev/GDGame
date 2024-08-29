using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour {
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text endGameResult;
    private void Awake() {
        exitButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    public void SetPopUp(string playerId) {
        gameObject.SetActive(true);
        if (playerId.Equals(Player.Instance.PlayerId)) {
            endGameResult.color = Color.green;
            endGameResult.text = "You're the WINNER!";
        }
        else {
            endGameResult.color = Color.red;
            endGameResult.text = "The winner is " + GameManager.Instance.getEnemyNameById(playerId) + "!";
        }
    }
}
