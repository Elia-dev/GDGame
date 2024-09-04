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
        //[SerializeField] private TMP_Text objectiveInfo;
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
                turn.color = Color.black;
                turn.text = "Is your turn!\n";
            }
            else {
                turn.color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(GameManager.Instance.getIdPlayingPlayer()), 255);
                turn.text = GameManager.Instance.getEnemyNameById(GameManager.Instance.getIdPlayingPlayer()) +
                            "'s turn!\n";
            }

            if (!_settingGame) {
                //Debug.Log("DIM " + userSpace.GetComponent<RectTransform>().rect.width );
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
                //objectiveInfo.gameObject.SetActive(true);
                //objectiveInfo.text = "Ojective: " + Player.Instance.ObjectiveCard.description;
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
        
            /*if (Input.GetKeyDown(KeyCode.Escape)) {
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (Canvas canvas in allCanvases)
                {
                    // Controlla se il canvas è in modalità Screen Space - Overlay
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        // Controlla se il Canvas è attivo e se ha GameObject attivi
                        if (canvas.gameObject.activeInHierarchy) {
                            return;
                        }
                    }
                }
                escMenu.SetActive(true);
            }*/
        }

        public void ShowTerritoryInfo(Territory territory) {
            //territoryInfo.gameObject.SetActive(true);
            //Territory territory = GameManager.Instance.AllTerritories.Find(terr => terr.id.Equals(id));
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