using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour {
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject circlePlayerColor;
    [SerializeField] private TMP_Text turn;
    [SerializeField] private GameObject clickHandler;
    [SerializeField] private TMP_Text allInfo;
    [SerializeField] private GameObject userSpace;
    private string _territoryInfo;
    /*[SerializeField] private TMP_Text territoryInfo;
    [SerializeField] private TMP_Text objectiveInfo;*/
    private static bool _settingGame = true;
    //private bool _playerBaseInfoSet = false;


    public static bool SettingGame {
        get => _settingGame;
        set => _settingGame = value;
    }

    void Start() {
        playerName.text = Player.Instance.Name;
        allInfo.gameObject.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(userSpace.GetComponent<RectTransform>().rect.width, 
                userSpace.GetComponent<RectTransform>().sizeDelta.y);
        //allInfo.GetComponent<LayoutElement>().preferredWidth = userSpace.GetComponent<Transform>().;
        //Debug.Log("Preferred Width " + allInfo.GetComponent<LayoutElement>().preferredWidth + 
                  //"\nRect Width " + userSpace.GetComponent<RectTransform>().rect.width);
        //circlePlayerColor.GetComponent<Image>().color = Utils.ColorCode(Player.Instance.ArmyColor, 255);
        //objectiveInfo.text = Player.Instance.ObjectiveCard.description;
    }

    void Update() {
        allInfo.text = "";
        if (Player.Instance.IsMyTurn) {
            turn.color = Color.black;
            turn.text = "Is your turn!";
        }
        else {
            turn.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(GameManager.Instance.getIdPlayingPlayer()), 255);
            turn.text = GameManager.Instance.getEnemyNameById(GameManager.Instance.getIdPlayingPlayer()) +
                        "'s turn!";
        }

        if (!_settingGame) {
            circlePlayerColor.gameObject.SetActive(true);
            circlePlayerColor.GetComponent<Image>().color = Utils.ColorCode(Player.Instance.ArmyColor, 255);
            //objectiveInfo.gameObject.SetActive(true);
            //objectiveInfo.text = "Ojective: " + Player.Instance.ObjectiveCard.description;
            allInfo.text += "Ojective: " + Player.Instance.ObjectiveCard.description;
        }
        
        if (!_settingGame && TerritoriesManagerUI.distributionPhase) {
            allInfo.text += "\n<u>Distribution Phase!</u>\nSelect your states and add " +
                            clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks of "
                            + Player.Instance.TanksAvailable + " still available";
        }
        else if (!_settingGame && TerritoriesManagerGamePhaseUI.ReinforcePhase) {
            allInfo.text += "\n<u>Reinforce Phase!</u>\nSelect your states and add " +
                            clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks";
        }
        else if (!_settingGame && TerritoriesManagerGamePhaseUI.Attackphase) {
            allInfo.text += "\n<u>Attack Phase!</u>\nAttack the enemies or move your army";
        }

        allInfo.text += "\n" + _territoryInfo;
    }

    public void ShowTerritoryInfo(Territory territory) {
        //territoryInfo.gameObject.SetActive(true);
        //Territory territory = GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
        if (territory is not null) {
            _territoryInfo = territory.name + $": state of the continent {territory.continent}, owned by the player" +
                             $"<color={GameManager.Instance.GetPlayerColor(territory.player_id)}>" +
                             $"{GameManager.Instance.getEnemyNameById(territory.player_id)}</color>.\n" +
                             $"On the territory there are {territory.num_tanks} army on it.";
            
        }
    }
    
    public void HideTerritoryInfo() {
        _territoryInfo = "";
    }
    
}