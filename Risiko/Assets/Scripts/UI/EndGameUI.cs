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

    void SetPopUp() {
        gameObject.SetActive(true);
    }
}

