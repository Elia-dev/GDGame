using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerUI : MonoBehaviour {
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text turn;
    [SerializeField] private GameObject clickHandler;
    [SerializeField] private TMP_Text turnInfo;
    [SerializeField] private GameObject UIcontainer;
    // Start is called before the first frame update
    void Start() {
        playerName.text = Player.Instance.Name;
        
    }

    // Update is called once per frame
    void Update() {
        if (Player.Instance.IsMyTurn) {
            turn.text = "IS YOUR TURN!";
        }

        //else
            //turn.text = Player.Instance.Name + "'S TURN!"; //DA CAMBIARE CON IL TURNO DEL PLAYER
            if (TerritoriesManagerUI.distributionPhase) {
                turnInfo.text = "DISTRIBUTION PHASE\nSelect your states and add " + Player.Instance.TanksAvailable +
                                " tanks";
            } //else if (clickHandler.GetComponent<TerritoriesManagerGamePhaseUI>().ReinforcePhase) {
                
            //}
    }
}
