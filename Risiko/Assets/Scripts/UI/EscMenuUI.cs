using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscMenuUI : MonoBehaviour {
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;

    private void Awake() {
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
        optionsButton.onClick.AddListener(() => volumeMenu.SetActive(true));
        exitButton.onClick.AddListener(() => exitMenu.SetActive(true));
    }

    public void BackForVolumeMenu() {
        volumeMenu.SetActive(false);
    }
    
    public void BackButtonForExitMenu() {
        exitMenu.SetActive(false);
    }

    public void ExitButtonForExitMenu() {
        Application.Quit();
    }
}
