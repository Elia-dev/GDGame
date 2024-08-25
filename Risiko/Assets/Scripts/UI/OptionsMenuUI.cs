using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour {
    [SerializeField] private Button backButton;

    private void Awake() 
    {
        backButton.onClick.AddListener(() => {
            Debug.Log("Exiting options menu...");
            SceneManager.LoadScene("MainMenu");
        });

        
    }

    private void Update() 
    {
        
    }
}