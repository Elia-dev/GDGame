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
    private static bool _settingGame = true;

    public static bool SettingGame {
        get => _settingGame;
        set => _settingGame = value;
    }

    // Start is called before the first frame update
    void Start() {
        playerName.text = Player.Instance.Name;
    }

    // Update is called once per frame
    void Update() {
        if (Player.Instance.IsMyTurn) 
            turn.text = "Is your turn!";
        else
            turn.text = GameManager.Instance.getEnemyNameById(GameManager.Instance.getIdPlayingPlayer()) + "'s turn!"; //DA CAMBIARE CON IL TURNO DEL PLAYER
        
        if (!_settingGame && TerritoriesManagerUI.distributionPhase) {
            turnInfo.text = "<u>Distribution Phase!</u>\nSelect your states and add " +
                            clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks";
        }
        else if (!_settingGame && clickHandler.GetComponent<TerritoriesManagerGamePhaseUI>().ReinforcePhase) {
            turnInfo.text = "<u>Reinforce Phase!</u>\nSelect your states and add " +
                            clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks";
        } else if (!_settingGame && clickHandler.GetComponent<TerritoriesManagerGamePhaseUI>().ReinforcePhase) {
            turnInfo.text = "<u>Attack Phase!</u>\nAttack the enemies or move your army";
        }
    }

    public void ShowTerritoryInfo(string id) {
        
    }
}