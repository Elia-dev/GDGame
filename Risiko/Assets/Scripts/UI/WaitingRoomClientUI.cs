using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomClientUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private TMP_Text PlayerList;
    [SerializeField] private TMP_Text LobbyID;
    [SerializeField] private GameObject PopUpDice;
    // Start is called before the first frame update
    void Start()
    {
        //COSE SERVER
        
        //Visualizzazione copdice lobby
        //LobbyID.text = "1518";
    }

    // Update is called once per frame
    void Update()
    {
        //COSE SERVER
        
        //Aggiornamento lista giocatori
        //PlayerList.text = "P1 P2 ...";
        
        //Quando l'HOST avvia il gioco
        //PopUpDice.SetActive(true);
    }

    private void Awake()
    {
        BackButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene("GameMenu");
        });
    }
}
