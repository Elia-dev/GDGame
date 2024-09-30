using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI;
using UnityEngine;

namespace businesslogic
{
    public class RequestHandler
    {
        private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();
        private string _request;
        public async Task HandleRequests(CancellationToken cancellationToken)
        {
            await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
            {
                if (!ClientManager.Instance.IsConnected())
                {
                    return;
                }

                if (message.Contains("LOBBY_ID:")) // Manage lobby_id request
                {
                    _request = RemoveRequest(message, "LOBBY_ID:");
                    GameManager.Instance.SetLobbyId(_request);
                }
                else if (message.Contains("CONNECTED_TO_LOBBY"))
                {
                    ClientManager.Instance.setIsConnectedToLobby(true);
                }
                else if (message.Contains("CONNECTION_REFUSED"))
                {
                    ClientManager.Instance.setIsConnectedToLobby(false);
                }
                else if (message.Contains("GAME_KILLED_BY_HOST"))
                {
                    GameManager.Instance.SetGameRunning(false);
                }
                else if (message.Contains("KICKED_FROM_GAME"))
                {
                    GameManager.Instance.SetGameRunning(false);
                }
                else if (message.Contains("SELECT_ALL_GAMES:"))
                {
                    _request = RemoveRequest(message, "SELECT_ALL_GAMES: ");
                    List<Lobby> lobbies = new List<Lobby>();

                    _request = _request.Trim('[', ']');
                    string[] parts = _request.Split(new string[] { ", " }, StringSplitOptions.None);

                       
                    for (int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = parts[i].Trim('\''); 
                    }
                    for (int i = 0; i < parts.Length; i += 3)
                    {
                        string lobbyId = parts[i];
                        string hostName = parts[i + 1]; 
                        int number = int.Parse(parts[i + 2]);
                        lobbies.Add(new Lobby(lobbyId, hostName, number));
                    }
                    MatchmakingManager.LoadAvailableLobbies(lobbies);
                }
                else if (message.Contains("GAME_STARTED_BY_HOST"))
                {
                    GameManager.Instance.SetGameWaitingToStart(false);
                }
                else if (message.Contains("REQUEST_NAME_UPDATE_PLAYER_LIST:"))
                {
                    GameManager.Instance.ResetPlayersName();
                    _request = RemoveRequest(message, "REQUEST_NAME_UPDATE_PLAYER_LIST: ");
                    string[] str= _request.Split(" ");
                    foreach (var name in str)
                    {
                        string cleanedName = name.Replace("[", "").Replace("]", "").Replace(",", "").Replace("'", "");
                        GameManager.Instance.AddPlayerName(cleanedName);
                    }
                
                }
                else if (message.Contains("GAME_ORDER:"))
                {
                    _request = RemoveRequest(message, "GAME_ORDER: ");
                    GameManager.Instance.setGame_order(_request);
                }
                else if(message.Contains("ID_NAMES_DICT"))
                {
                    _request = RemoveRequest(message, "ID_NAMES_DICT: ");
                    string[] pairs = _request.Split(", ");
                
                    foreach (string pair in pairs)
                    {
                        string[] parts = pair.Split('-');
                        GameManager.Instance.AddPlayerToLobbyDict(parts[0], parts[1]);
                    }
                }
                else if (message.Contains("PLAYER_KILLED_BY:"))
                {
                    _request = RemoveRequest(message, "PLAYER_KILLED_BY: ");
                    GameManager.Instance.SetKillerId(_request);
                }
                else if (message.Contains("EXTRACTED_NUMBER:"))
                {
                    _request = RemoveRequest(message, "EXTRACTED_NUMBER: ");
                    int extractedNumber = int.Parse(_request);
                    GameManager.Instance.SetExtractedNumber(extractedNumber);
                }
                else if (message.Contains("GAME_ORDER_EXTRACTED_NUMBERS:"))
                {
                    _request = RemoveRequest(message, "GAME_ORDER_EXTRACTED_NUMBERS: ");
                    GameManager.Instance.SetGameOrderExtractedNumbers(_request);
                }
                else if (message.Contains("BOT_NUMBER:"))
                {
                    Debug.Log("Server_Request: BOT_NUMBER");
                    _request = RemoveRequest(message, "BOT_NUMBER: ");
                    int botNumber = int.Parse(_request);
                    Debug.Log("Numero bot settato: " + botNumber);
                    GameManager.Instance.SetBotNumber(botNumber); 
                }
                else if (message.Contains("PLAYER_ID:"))
                {
                    _request = RemoveRequest(message, "PLAYER_ID: ");
                    Player.Instance.PlayerId = _request;
                }
                else if (message.Contains("IS_YOUR_TURN:"))
                {
                    _request = RemoveRequest(message, "IS_YOUR_TURN: ");
                    Player.Instance.IsMyTurn = _request.Equals("TRUE");
                }
                else if (message.Contains("PLAYER_TURN"))
                {
                    _request = RemoveRequest(message, "PLAYER_TURN: ");
                    GameManager.Instance.SetIdPlayingPlayer(_request);
                }
                else if (message.Contains("AVAILABLE_COLORS:"))
                {
                    _request = RemoveRequest(message, "AVAILABLE_COLORS: ");
                    string[] str= _request.Split(" ");
                
                    foreach (var color in str)
                    {
                        string cleanedColor = color.Replace("[", "").Replace("]", "").Replace(",", "");
                    
                        GameManager.Instance.AddAvailableColor(cleanedColor);
                    }
                }
                else if (message.Contains("ID_COLORS_DICT"))
                {
                    _request = RemoveRequest(message, "ID_COLORS_DICT: ");
                    string[] pairs = _request.Split(", ");
                
                    foreach (string pair in pairs)
                    {
                        string[] parts = pair.Split('-');
                        GameManager.Instance.AddPlayerColor(parts[0], parts[1]);
                    }
                
                }
                else if (message.Contains("INITIAL_ARMY_NUMBER:"))
                {
                    _request = RemoveRequest(message, "INITIAL_ARMY_NUMBER: ");
                    int armyNumber = int.Parse(_request);
                
                    Player.Instance.TanksNum = armyNumber;
                    Player.Instance.TanksPlaced = Player.Instance.Territories.Count;
                    Player.Instance.TanksAvailable = armyNumber - Player.Instance.TanksPlaced;
                
                }
                else if (message.Contains("OBJECTIVE_CARD_ASSIGNED:"))
                {
                    _request = RemoveRequest(message, "OBJECTIVE_CARD_ASSIGNED: ");
                    Player.Instance.ObjectiveCard = Objective.FromJson(_request);
                
                }
                else if (message.Contains("TERRITORIES_CARDS_ASSIGNED:"))
                {
                    _request = RemoveRequest(message, "TERRITORIES_CARDS_ASSIGNED: ");
                    Player.Instance.Territories = JsonConvert.DeserializeObject<List<Territory>>(_request);
                }
                else if (message.Contains("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN:"))
                {
                    _request = RemoveRequest(message, "NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: ");
                    int armyNumber = int.Parse(_request);
                    Player.Instance.TanksAvailable = armyNumber;
                    Player.Instance.TanksNum += armyNumber;
                }
                else if (message.Contains("PREPARATION_PHASE_TERMINATED"))
                {
                    GameManager.Instance.SetGamePhase(true); 
                    GameManager.Instance.SetPreparationPhase(false);
                }
                else if (message.Contains("SEND_TERRITORIES_TO_ALL"))
                {
                    _request = RemoveRequest(message, "SEND_TERRITORIES_TO_ALL: ");
                    GameManager.Instance.AllTerritories = JsonConvert.DeserializeObject<List<Territory>>(_request);
                }
                else if (message.Contains("UNDER_ATTACK"))
                {
                    _request = RemoveRequest(message, "UNDER_ATTACK: ");
                    var matches = Regex.Matches(_request, @"\[(.*?)\]");
                    GameManager.Instance.SetEnemyExtractedNumbers(
                        matches[0].Groups[1].Value
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray()
                    );
                    GameManager.Instance.SetMyExtractedNumbers(
                        matches[1].Groups[1].Value
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray()
                    );
                    _request = _request.Replace(" ", "");
                
                    string[] parts = _request.Split(',');

                    string[] terIds = parts[1].Split('-');
                    string attackerTerID = terIds[0];
                    string defenderTerID = terIds[1];
                
                    string[] armyNums = parts[2].Split('-');
                    int attackerArmyNum = int.Parse(armyNums[0]);
                    int defenderArmyNum = int.Parse(armyNums[1]);
                
                
                    GameManager.Instance.SetEnemyArmyNum(attackerArmyNum);
                    GameManager.Instance.SetMyArmyNum(defenderArmyNum);

                    GameManager.Instance.SetEnemyTerritoy(GameManager.Instance.AllTerritories.Find(x => x.id == attackerTerID));
                    GameManager.Instance.SetMyTerritory(Player.Instance.Territories.Find(x => x.id == defenderTerID));
               
                    GameManager.Instance.SetImUnderAttack(true);
                    GameManager.Instance.SetImAttacking(false);
                }
                else if (message.Contains("ATTACKER_ALL_EXTRACTED_DICE"))
                {
                    _request = RemoveRequest(message, "ATTACKER_ALL_EXTRACTED_DICE: ");
                    var matches = Regex.Matches(_request, @"\[(.*?)\]");
                
                    GameManager.Instance.SetMyExtractedNumbers(
                        matches[0].Groups[1].Value
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray()
                    );
            
                    GameManager.Instance.SetEnemyExtractedNumbers(
                        matches[1].Groups[1].Value
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray()
                    );
                    GameManager.Instance.SetImAttacking(true);
                    GameManager.Instance.SetImUnderAttack(false);
                }
                else if (message.Contains("ATTACK_FINISHED_FORCE_UPDATE"))
                {
                    try 
                    {
                        List<Territory> toRemove = new List<Territory>();
                        List<Territory> toAdd = new List<Territory>();

                        for(int i = 0; i <  GameManager.Instance.AllTerritories.Count; i++)
                        {
                            Territory terr = GameManager.Instance.AllTerritories[i];
                            Territory playerTerr = Player.Instance.Territories.Find(x => x.id == terr.id);
                            if (playerTerr is not null && terr.player_id != Player.Instance.PlayerId)
                            {
                                toRemove.Add(playerTerr);
                                GameManager.Instance.SetWinnerBattleId(GameManager.Instance.GetEnemyTerritory().player_id); // Territory won by the enemy
                            }
                            else if (playerTerr is null && terr.player_id == Player.Instance.PlayerId)
                            {
                                //   Debug.Log("Il terr: " + terr.name + " non è nella tua lista territori ma in realtà ti appartiene");
                                toAdd.Add(terr);
                                GameManager.Instance.SetWinnerBattleId(Player.Instance.PlayerId); // Territory won by the player

                            }
                            else if (playerTerr is null && terr.player_id != Player.Instance.PlayerId)
                            {
                                Debug.Log("skip this territory");
                            }
                            else 
                            {
                                Player.Instance.Territories.Find(x => x.id == terr.id).num_tanks = terr.num_tanks;
                            }
                        }
                    
                        foreach (var terrToRemove in toRemove)
                        {
                            Player.Instance.Territories.Remove(terrToRemove);
                        }

                        foreach (var terrToAdd in toAdd)
                        {
                            Player.Instance.Territories.Add(terrToAdd);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Error: " + ex.Message);
                    }

                    GameManager.Instance.SetForceUpdateAfterAttack(true);
                }
                else if (message.Contains("WINNER"))
                {
                    _request = RemoveRequest(message, "WINNER: ");
                    GameManager.Instance.SetWinnerGameId(_request);
                }
                else
                {
                    Debug.Log("HANDLER: request not manageable: " + message);
                }
            }
        }

        public void AddRequest(string clientId, string message)
        {
            _queue.Writer.TryWrite((clientId, message));
        }
    
        private string RemoveRequest(string source, string request)
        {
            string value = source.Replace(request, "");
            return value;
        }
    }
}
