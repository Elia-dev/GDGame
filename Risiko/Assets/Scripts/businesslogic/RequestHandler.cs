using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;

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
            
            if(message.Contains("LOBBY_ID:")) // Manage lobby_id request
            {
                Debug.Log("Ricevuta richiesta: LOBBY_ID " + message);
                _request = RemoveRequest(message, "LOBBY_ID:");
                //Debug.Log("INFO ESTRAPOLATA:" + _request);
                GameManager.Instance.SetLobbyId(_request);
            }
            else if (message.Contains("GAME_STARTED_BY_HOST"))
            {
                GameManager.Instance.SetGameWaitingToStart(false);
            }
            else if (message.Contains("REQUEST_NAME_UPDATE_PLAYER_LIST:"))
            {
                Debug.Log("Server_Request: REQUEST_NAME_UPDATE_PLAYER_LIST");
                
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
                Debug.Log("Server_Request: GAME_ORDER");
                _request = RemoveRequest(message, "GAME_ORDER: ");
                GameManager.Instance.setGame_order(_request);
            }
            else if (message.Contains("EXTRACTED_NUMBER:"))
            {
                Debug.Log("Server_Request: EXTRACTED_NUMBER");
                _request = RemoveRequest(message, "EXTRACTED_NUMBER: ");
                int extractedNumber = int.Parse(_request);
                
                GameManager.Instance.SetExtractedNumber(extractedNumber);
            }
            else if (message.Contains("GAME_ORDER_EXTRACTED_NUMBERS:"))
            {
                Debug.Log("Server_Request: GAME_ORDER_EXTRACTED_NUMBERS");
                _request = RemoveRequest(message, "GAME_ORDER_EXTRACTED_NUMBERS: ");
                GameManager.Instance.SetGameOrderExtractedNumbers(_request);
            }
            else if (message.Contains("PLAYER_ID:"))
            {
                Debug.Log("Server_Request: PLAYER_ID");
                _request = RemoveRequest(message, "PLAYER_ID: ");
                Player.Instance.PlayerId = _request;
            }
            else if (message.Contains("IS_YOUR_TURN:"))
            {
                Debug.Log("Server_Request: IS_YOUR_TURN");
                Debug.Log(message);
                _request = RemoveRequest(message, "IS_YOUR_TURN: ");
                Player.Instance.IsMyTurn = _request.Equals("TRUE");
            }
            else if (message.Contains("AVAILABLE_COLORS:"))
            {
                Debug.Log("Server_Request: AVAILABLE_COLORS");
                _request = RemoveRequest(message, "AVAILABLE_COLORS: ");
                Debug.Log("RIMOSSA RICHIESTA:" + _request);
                string[] str= _request.Split(" ");
                Debug.Log("ESEGUITO SPLIT: ");
                foreach (var stringa in str)
                {
                    Debug.Log(stringa);
                }
                
                foreach (var color in str)
                {
                    string cleanedColor = color.Replace("[", "").Replace("]", "").Replace(",", "");
                    Debug.Log("COLORE PULITO: " + cleanedColor);
                    GameManager.Instance.AddAvailableColor(cleanedColor);
                }
            }
            else if (message.Contains("INITIAL_ARMY_NUMBER:"))
            {
                Debug.Log("Server_Request: INITIAL_ARMY_NUMBER");
                _request = RemoveRequest(message, "INITIAL_ARMY_NUMBER: ");
                int armyNumber = int.Parse(_request);
                Debug.Log("ArmyNumber parsed: " + armyNumber);
                Player.Instance.TanksNum = armyNumber;
                Player.Instance.TanksPlaced = Player.Instance.Territories.Count;
                Player.Instance.TanksAvailable = armyNumber - Player.Instance.TanksPlaced;
                
            }
            else if (message.Contains("OBJECTIVE_CARD_ASSIGNED:"))
            {
                Debug.Log("Server_Request: OBJECTIVE_CARD_ASSIGNED");
                Debug.Log("Message + JSON: " + message);
                _request = RemoveRequest(message, "OBJECTIVE_CARD_ASSIGNED: ");
                Debug.Log("JSON ONLY: " + _request);
                
                Player.Instance.ObjectiveCard = Objective.FromJson(_request);
                Debug.Log("Json unpacked TOSTRING: " + Player.Instance.ObjectiveCard);
            }
            else if (message.Contains("TERRITORIES_CARDS_ASSIGNED:"))
            {
                Debug.Log("Server_Request: TERRITORIES_CARDS_ASSIGNED");
                Debug.Log("Message + JSON: " + message);
                _request = RemoveRequest(message, "TERRITORIES_CARDS_ASSIGNED: ");
                Debug.Log("JSON ONLY: " + _request);
               Player.Instance.Territories = JsonConvert.DeserializeObject<List<Territory>>(_request);
               Debug.Log("Json unpacked TOSTRING: " + Player.Instance.Territories);
            }
            else if (message.Contains("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN:"))
            {
                Debug.Log("Server_Request: NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN");
                _request = RemoveRequest(message, "NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: ");
                int armyNumber = int.Parse(_request);
                Player.Instance.TanksAvailable = armyNumber;
                Player.Instance.TanksNum += armyNumber;
                Debug.Log("Tanks Available server: " + Player.Instance.TanksAvailable);
            }
            else if (message.Contains("PREPARATION_PHASE_TERMINATED"))
            {
                Debug.Log("Server_Request: PREPARATION_PHASE_TERMINATED");
                GameManager.Instance.SetGamePhase(true); 
                GameManager.Instance.SetPreparationPhase(false);
            }
            else if (message.Contains("SEND_TERRITORIES_TO_ALL"))
            {
                Debug.Log("Server_Request: SEND_TERRITORIES_TO_ALL");
                _request = RemoveRequest(message, "SEND_TERRITORIES_TO_ALL: ");
                GameManager.Instance.AllTerritories = JsonConvert.DeserializeObject<List<Territory>>(_request);
                Debug.Log("Lista di tutti i territori RequestHandler: ");
                foreach (var terr in GameManager.Instance.AllTerritories) {
                    Debug.Log(terr);
                }
            }
            else if (message.Contains("UNDER_ATTACK"))
            {
                //UNDER_ATTACK: attackerId, attacker_ter_id-defender_ter_id, attacker_army_num-defender_army_num
                Debug.Log("Server_Request: UNDER_ATTACK");
                GameManager.Instance.setImUnderAttack(true);
                _request = RemoveRequest(message, "UNDER_ATTACK: ");
                
                _request = _request.Replace(" ", "");
                
                string[] parts = _request.Split(',');
                string attackerId = parts[0];
                
                string[] terIds = parts[1].Split('-');
                string attacker_ter_id = terIds[0];
                string defender_ter_id = terIds[1];
                
                string[] armyNums = parts[2].Split('-');
                int attacker_army_num = int.Parse(armyNums[0]);
                int defender_army_num = int.Parse(armyNums[1]);
                
                GameManager.Instance.setEnemyAttackerArmyNum(attacker_army_num);
                GameManager.Instance.setMyArmyNumToDefende(defender_army_num);
                // Mi salvo quale è lo stato che mi sta attaccando, e di chi è
                foreach (var terr in GameManager.Instance.AllTerritories)
                {
                    if (attacker_ter_id == terr.id)
                    {
                        GameManager.Instance.getEnemyAttackerTerritory().id = attacker_ter_id;
                        GameManager.Instance.getEnemyAttackerTerritory().name = terr.name;
                        GameManager.Instance.getEnemyAttackerTerritory().continent = terr.continent;
                        GameManager.Instance.getEnemyAttackerTerritory().node = terr.node;
                        GameManager.Instance.getEnemyAttackerTerritory().num_tanks = terr.num_tanks;
                        GameManager.Instance.getEnemyAttackerTerritory().description = terr.description;
                        GameManager.Instance.getEnemyAttackerTerritory().function = terr.function;
                        GameManager.Instance.getEnemyAttackerTerritory().image = terr.image;
                        GameManager.Instance.getEnemyAttackerTerritory().player_id = terr.player_id;
                    }
                }
                
                // Mi salvo quale mio stato sta venendo attaccato
                foreach (var terr in GameManager.Instance.AllTerritories)
                {
                    if (defender_ter_id == terr.id)
                    {
                        GameManager.Instance.getMyTerritoryUnderAttack().id = defender_ter_id;
                        GameManager.Instance.getMyTerritoryUnderAttack().name = terr.name;
                        GameManager.Instance.getMyTerritoryUnderAttack().continent = terr.continent;
                        GameManager.Instance.getMyTerritoryUnderAttack().node = terr.node;
                        GameManager.Instance.getMyTerritoryUnderAttack().num_tanks = terr.num_tanks;
                        GameManager.Instance.getMyTerritoryUnderAttack().description = terr.description;
                        GameManager.Instance.getMyTerritoryUnderAttack().function = terr.function;
                        GameManager.Instance.getMyTerritoryUnderAttack().image = terr.image;
                        GameManager.Instance.getMyTerritoryUnderAttack().player_id = terr.player_id;
                    }
                }
                
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
        //_queue.Writer.WriteAsync((clientId, message));
    }
    
    private string RemoveRequest(string source, string request)
    {
        string value = source.Replace(request, "");
        //Debug.Log("VALORE CALCOLATO:" + value);
        return value;
    }
}
