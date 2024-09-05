using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


        public static bool SettingGame {
            get => _settingGame;
            set => _settingGame = value;
        }
    
        public static bool DistributionPhase {
            get => _distributionPhase;
            set => _distributionPhase = value;
        }

        public static bool ReinforcePhase {
            get => _reinforcePhase;
            set => _reinforcePhase = value;
        }

        public static bool AttackPhase {
            get => _attackPhase;
            set => _attackPhase = value;
        }

        void Start() {
            playerName.text = Player.Instance.Name;
        }

        void Update() {
            allInfo.text = "";
            if (Player.Instance.IsMyTurn) {
                turn.color = new Color32(255, 216, 0, 255);
                turn.text = "Is your turn!\n";
            }
            else {
                turn.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(GameManager.Instance.getIdPlayingPlayer()), 255);
                turn.text = GameManager.Instance.getEnemyNameById(GameManager.Instance.getIdPlayingPlayer()) +
                            "'s turn!\n";
            }

            if (!_settingGame) {
                if(!_dimensionSetted) {
                    _dimensionSetted = true;
                    //Impostazione dimensioni text
                    allInfo.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        userSpace.GetComponent<RectTransform>().rect.width - 30,
                        allInfo.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    turn.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        userSpace.GetComponent<RectTransform>().rect.width - 30,
                        turn.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                }
                circlePlayerColor.gameObject.SetActive(true);
                //Colore giocaore
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
            //Sostituzione sigla continente con nome
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
                                 $"<color={GameManager.Instance.GetPlayerColor(territory.player_id)}>" +
                                 $"{GameManager.Instance.getEnemyNameById(territory.player_id)}</color>.\n" +
                                 $"On the territory there are <b>{territory.num_tanks}</b> army on it.\n";
            
            }
        }
    
        public void HideTerritoryInfo() {
            _territoryInfo = "";
        }
    }
}