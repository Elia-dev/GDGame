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
    private GameManager gm = GameManager.Instance;
    public async Task HandleRequests(CancellationToken cancellationToken)
    {
        await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            
            if(message.Contains("LOBBY_ID:")) // Manage lobby_id request
            {
                Debug.Log("Ricevuta richiesta: LOBBY_ID " + message);
                _request = RemoveRequest(message, "LOBBY_ID:");
                //Debug.Log("INFO ESTRAPOLATA:" + _request);
                ClientManager cm = ClientManager.Instance;
                cm.SetLobbyId(_request);
            }
            else if (message.Contains("GAME_STARTED_BY_HOST"))
            {
                GameManager.Instance.SetGameWaitingToStart(false);
            }
            else if (message.Contains("REQUEST_NAME_UPDATE_PLAYER_LIST:"))
            {
                Debug.Log("Server_Request: REQUEST_NAME_UPDATE_PLAYER_LIST");
                
                gm.ResetPlayersName();
                _request = RemoveRequest(message, "REQUEST_NAME_UPDATE_PLAYER_LIST: ");
                //Debug.Log("RIMOSSA RICHIESTA:" + _request);
                string[] str= _request.Split(" ");
                //Debug.Log("ESEGUITO SPLIT: ");
                foreach (var name in str)
                {
                    string cleanedName = name.Replace("[", "").Replace("]", "").Replace(",", "").Replace("'", "");
                    /*
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (name[i] == '[' || name[i] == ']' || name[i] == ',')
                        {
                            name.Remove(i);
                        }
                    }*/
                    
                    //Debug.Log(name);
                    gm.AddPlayerName(cleanedName);
                }
                //Debug.Log("Lista players: " + cm.NamePlayersTemporaneo.ToString());
            }
            else if (message.Contains("GAME_ORDER:"))
            {
                Debug.Log("Server_Request: GAME_ORDER");
                _request = RemoveRequest(message, "GAME_ORDER: ");
                gm.setGame_order(_request);
            }
            else if (message.Contains("EXTRACTED_NUMBER:"))
            {
                Debug.Log("Server_Request: EXTRACTED_NUMBER");
                _request = RemoveRequest(message, "EXTRACTED_NUMBER: ");
                int extractedNumber = int.Parse(_request);
                
                gm.SetExtractedNumber(extractedNumber);
            }
            else if (message.Contains("GAME_ORDER_EXTRACTED_NUMBERS:"))
            {
                Debug.Log("Server_Request: GAME_ORDER_EXTRACTED_NUMBERS");
                _request = RemoveRequest(message, "GAME_ORDER_EXTRACTED_NUMBERS: ");
                gm.SetGameOrderExtractedNumbers(_request);
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
                    gm.AddAvailableColor(cleanedColor);
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
                Debug.Log("Json unpacked TOSTRING: " + Player.Instance.ObjectiveCard.ToString());
            }
            else if (message.Contains("TERRITORIES_CARDS_ASSIGNED:"))
            {
                Debug.Log("Server_Request: TERRITORIES_CARDS_ASSIGNED");
                Debug.Log("Message + JSON: " + message);
                _request = RemoveRequest(message, "TERRITORIES_CARDS_ASSIGNED: ");
                Debug.Log("JSON ONLY: " + _request);
               Player.Instance.Territories = JsonConvert.DeserializeObject<List<Territory>>(_request);
               Debug.Log("Json unpacked TOSTRING: " + Player.Instance.Territories.ToString());
            }
            else if (message.Contains("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN:"))
            {
                Debug.Log("Server_Request: NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN");
                _request = RemoveRequest(message, "NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: ");
                int armyNumber = int.Parse(_request);
                Player.Instance.TanksAvailable = armyNumber;
                Player.Instance.TanksNum += armyNumber;
            }
            else if (message.Contains("RECEIVED_REQUEST_TERRITORY_INFO:"))
            {
                Debug.Log("Server_Request: RECEIVED_REQUEST_TERRITORY_INFO");
                _request = RemoveRequest(message, "RECEIVED_REQUEST_TERRITORY_INFO: ");
                GameManager.Instance.puppetState = JsonConvert.DeserializeObject<Territory>(_request);
            }
            else if (message.Contains("PREPARATION_PHASE_TERMINATED"))
            {
                GameManager.Instance.setGamePhase(true); 
                GameManager.Instance.setPreparationPhase(false);
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
