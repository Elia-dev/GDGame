using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour {
    [SerializeField] private Button exitButton;
    private void Awake() {
        exitButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    void SetPopUp() {
        
    }
}
