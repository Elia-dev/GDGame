using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerUI : MonoBehaviour {
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text turn;
    [SerializeField] private GameObject clickHandler;
    [SerializeField] private TMP_Text turnInfo;
    [SerializeField] private GameObject uiContainer;
    // Start is called before the first frame update
    void Start() {
        playerName.text = Player.Instance.Name;
        RectTransform temp = GameObject.Find("MapSpace").GetComponent<RectTransform>();
        Debug.Log("Schermo: " + Screen.width);
        float width = (float)(Screen.width * 4) / 5;
        Debug.Log("Width Map: " + width);
        temp.rect.Set(temp.rect.x, temp.rect.y, width, temp.rect.height);
        temp = GameObject.Find("UserSpace").GetComponent<RectTransform>();
        width = (float)(Screen.width * 1) / 5;
        temp.rect.Set(temp.rect.x, temp.rect.y, width, temp.rect.height);
        Debug.Log("Width Map: " + width);
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
