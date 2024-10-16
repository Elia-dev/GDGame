using System;
using businesslogic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace UI
{
    public class GameManagerUI : MonoBehaviour {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject circlePlayerColor;
        [SerializeField] private TMP_Text turn;
        [SerializeField] private GameObject clickHandler;
        [SerializeField] private TMP_Text allInfo;
        [SerializeField] private GameObject userSpace;
        
        private string _territoryInfo;
        private static bool _settingGame = true;
        private bool _dimensionSetted = false;
        private static bool _distributionPhase = false;
        private static bool _reinforcePhase = false;
        private static bool _attackPhase = false;
        private static bool _thisIsTheEnd = false;
        public static bool ThisIsTheEnd {
            set => _thisIsTheEnd = value;
        }

        public static bool SettingGame {
            set => _settingGame = value;
        }
    
        public static bool DistributionPhase {
            set => _distributionPhase = value;
        }

        public static bool ReinforcePhase {
            set => _reinforcePhase = value;
        }

        public static bool AttackPhase {
            set => _attackPhase = value;
        }

        void Start() {
            _settingGame = true;
            _distributionPhase = false;
            _reinforcePhase = false;
            _attackPhase = false;
            _thisIsTheEnd = false;
            playerName.text = Player.Instance.Name;
        }

        void Update() {
            if (!GameManager.Instance.GetGameRunning() || !GameManager.Instance.GetWinnerGameId().Equals("") || _thisIsTheEnd) {
                //Debug.Log("Gioco finito, faccio return in GameManagerUI");
                return;
            }
            
            allInfo.text = "";
            if (Player.Instance.IsMyTurn) {
                turn.color = new Color32(255, 216, 0, 255);
                turn.text = "Is your turn!\n";
            }
            else {
                turn.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(GameManager.Instance.GetIdPlayingPlayer()), 255);
                turn.text = GameManager.Instance.GetEnemyNameById(GameManager.Instance.GetIdPlayingPlayer()) +
                            "'s turn!\n";
            }

            if (!_settingGame) {
                if(!_dimensionSetted) {
                    _dimensionSetted = true;
                    allInfo.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        userSpace.GetComponent<RectTransform>().rect.width - 30,
                        allInfo.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    turn.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        userSpace.GetComponent<RectTransform>().rect.width - 30,
                        turn.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                }
                circlePlayerColor.gameObject.SetActive(true);
                circlePlayerColor.GetComponent<Image>().color = Utils.ColorCode(Player.Instance.ArmyColor, 255);
                allInfo.text += "\n<b>Objective</b>: " + Player.Instance.ObjectiveCard.description + "\n";
            }
        
            if (!_settingGame && _distributionPhase) {
                allInfo.text += "\n<b>Distribution Phase!</b>\nSelect your states and add " +
                                clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks of "
                                + Player.Instance.TanksAvailable + " still available\n";
            }
            else if (!_settingGame && _reinforcePhase) {
                allInfo.text += "\n<b>Reinforce Phase!</b>\nSelect your states and add " +
                                clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>().ArmyNumber + " tanks\n";
            }
            else if (!_settingGame && _attackPhase) {
                allInfo.text += "\n<b>Attack Phase!</b>\nAttack the enemies or move your army\n";
            }
            else
                allInfo.text += "\nWaiting for other players\n";

            allInfo.text += "\n" + _territoryInfo;
        }

        public void ShowTerritoryInfo(Territory territory) {
            if (territory is not null) {
                string continent = territory.continent;
                switch (continent) {
                    case "AF":
                        continent = "Africa";
                        break;
                    case "AS": 
                        continent = "Asia";
                        break;
                    case "EU":
                        continent = "Europe";
                        break;
                    case "NA":
                        continent = "North America";
                        break;
                    case "OC":
                        continent = "Oceania";
                        break;
                    case "SA":
                        continent = "South America";
                        break;
                    default:
                        continent = "Unknown";
                        break;
                }
                _territoryInfo = "\n<b>" + territory.name + $"</b>: state of the continent {continent}, owned by the player " +
                                 $"<b><color=#{ColorUtility.ToHtmlStringRGB(Utils.ColorCode(GameManager.Instance.GetPlayerColor(territory.player_id), 255))}>" +
                                 $"{GameManager.Instance.GetEnemyNameById(territory.player_id)}</color></b>.\n" +
                                 $"On the territory there are <b>{territory.num_tanks}</b> army on it.\n";
            
            }
        }
        public void HideTerritoryInfo() {
            _territoryInfo = "";
        }
    }
}
