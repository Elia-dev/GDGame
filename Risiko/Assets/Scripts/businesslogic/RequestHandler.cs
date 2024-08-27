using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
            else if(message.Contains("ID_NAMES_DICT"))
            {
                Debug.Log("Server_Request: ID_NAMES_DICT");
                _request = RemoveRequest(message, "ID_NAMES_DICT: ");
                string[] pairs = _request.Split(", ");
                
                foreach (string pair in pairs)
                {
                    // Divide ogni coppia id-nome usando il trattino "-"
                    string[] parts = pair.Split('-');
                    GameManager.Instance.AddPlayerToLobbyDict(parts[0], parts[1]);
                }
                
                Debug.Log("Aggiunti tutti i nomi al dizionario");

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
            else if (message.Contains("PLAYER_TURN"))
            {
                Debug.Log("Server_Request: PLAYER_TURN");
                _request = RemoveRequest(message, "PLAYER_TURN: ");
                GameManager.Instance.setIdPlayingPlayer(_request);
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
            else if (message.Contains("ID_COLORS_DICT"))
            {
                Debug.Log("Server_Request: ID_COLORS_DICT");
                _request = RemoveRequest(message, "ID_COLORS_DICT: ");
                string[] pairs = _request.Split(", ");
                
                foreach (string pair in pairs)
                {
                    string[] parts = pair.Split('-');
                    GameManager.Instance.AddPlayerColor(parts[0], parts[1]);
                }
                Debug.Log("Aggiunti tutti i colori al dizionario");
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
            }
            else if (message.Contains("UNDER_ATTACK"))
            {
                //UNDER_ATTACK: attackerId, attacker_ter_id-defender_ter_id, attacker_army_num-defender_army_num
                Debug.Log("Server_Request: UNDER_ATTACK");
                GameManager.Instance.setImUnderAttack(true);
                GameManager.Instance.setImAttacking(false);
                _request = RemoveRequest(message, "UNDER_ATTACK: ");
                Debug.Log("Richiesta pulita: " + _request);
                
                var matches = Regex.Matches(_request, @"\[(.*?)\]");
        
                Debug.Log("Prova ad estrarre le parentesi quadre");
                // Estrai i numeri dalla prima lista e mettili in un array
                GameManager.Instance.setEnemyExtractedNumbers(
                        matches[0].Groups[1].Value
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray()
                        );
            
                    // Estrai i numeri dalla seconda lista e mettili in un array
                GameManager.Instance.setMyExtractedNumbers(
                    matches[1].Groups[1].Value
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray()
                        );
                
                _request = _request.Replace(" ", "");
                
                string[] parts = _request.Split(',');
                string attackerId = parts[0];
                
                Debug.Log("AttackerId: " + attackerId);
                
                string[] terIds = parts[1].Split('-');
                string attacker_ter_id = terIds[0];
                string defender_ter_id = terIds[1];
                
                Debug.Log("attacker_ter_id: " + attacker_ter_id);
                Debug.Log("defender_ter_id: " + defender_ter_id);
                
                string[] armyNums = parts[2].Split('-');
                int attacker_army_num = int.Parse(armyNums[0]);
                int defender_army_num = int.Parse(armyNums[1]);
                
                Debug.Log("attacker_army_num: " + attacker_army_num);
                Debug.Log("defender_army_num: " + defender_army_num);
                
                GameManager.Instance.setEnemyArmyNum(attacker_army_num);
                GameManager.Instance.setMyArmyNum(defender_army_num);

                GameManager.Instance.setEnemyTerritoy(GameManager.Instance.AllTerritories.Find(x => x.id == attacker_ter_id));
                
                /*
                // Mi salvo quale è lo stato che mi sta attaccando, e di chi è
                foreach (var terr in GameManager.Instance.AllTerritories)
                {
                    if (attacker_ter_id == terr.id)
                    {
                        GameManager.Instance.getEnemyTerritory().id = attacker_ter_id;
                        GameManager.Instance.getEnemyTerritory().name = terr.name;
                        GameManager.Instance.getEnemyTerritory().continent = terr.continent;
                        GameManager.Instance.getEnemyTerritory().node = terr.node;
                        GameManager.Instance.getEnemyTerritory().num_tanks = terr.num_tanks;
                        GameManager.Instance.getEnemyTerritory().description = terr.description;
                        GameManager.Instance.getEnemyTerritory().function = terr.function;
                        GameManager.Instance.getEnemyTerritory().image = terr.image;
                        GameManager.Instance.getEnemyTerritory().player_id = terr.player_id;
                    }
                }
                */
                
                GameManager.Instance.setMyTerritory(Player.Instance.Territories.Find(x => x.id == defender_ter_id));
                /*
                
                // Mi salvo quale mio stato sta venendo attaccato
                foreach (var terr in GameManager.Instance.AllTerritories)
                {
                    if (defender_ter_id == terr.id)
                    {
                        GameManager.Instance.getMyTerritory().id = defender_ter_id;
                        GameManager.Instance.getMyTerritory().name = terr.name;
                        GameManager.Instance.getMyTerritory().continent = terr.continent;
                        GameManager.Instance.getMyTerritory().node = terr.node;
                        GameManager.Instance.getMyTerritory().num_tanks = terr.num_tanks;
                        GameManager.Instance.getMyTerritory().description = terr.description;
                        GameManager.Instance.getMyTerritory().function = terr.function;
                        GameManager.Instance.getMyTerritory().image = terr.image;
                        GameManager.Instance.getMyTerritory().player_id = terr.player_id;
                    }
                }
                */
                Debug.Log("Territorio nemico che mi attacca: " +  GameManager.Instance.getEnemyTerritory().name);
                Debug.Log("Il mio territorio sotto attacco: " + GameManager.Instance.getMyTerritory().name);
                Debug.Log("Numero armate che il nemico sta usando: " + GameManager.Instance.GetEnemyArmyNum());
                Debug.Log("Numero armate che uso per difendermi: " + GameManager.Instance.getMyArmyNum());
            }
            else if (message.Contains("ATTACKER_ALL_EXTRACTED_DICE"))
            {
                Debug.Log("Server_Request: ATTACKER_ALL_EXTRACTED_DICE");
                _request = RemoveRequest(message, "ATTACKER_ALL_EXTRACTED_DICE: ");
                var matches = Regex.Matches(_request, @"\[(.*?)\]");
        
                Debug.Log("Prova ad estrarre le parentesi quadre");
                // Estrai i numeri dalla prima lista e mettili in un array
                GameManager.Instance.setMyExtractedNumbers(
                    matches[0].Groups[1].Value
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray()
                );
            
                // Estrai i numeri dalla seconda lista e mettili in un array
                GameManager.Instance.setEnemyExtractedNumbers(
                    matches[1].Groups[1].Value
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray()
                );
            }
            else if (message.Contains("ATTACK_FINISHED_FORCE_UPDATE"))
            {
                Debug.Log("Server_Request: ATTACK_FINISHED_FORCE_UPDATE");
                //Debug.Log("NOTA: IO SONO " + Player.Instance.Name + " con id=" + Player.Instance.PlayerId);
              //  Debug.Log("Sto per scorrere " + GameManager.Instance.AllTerritories.Count + " territori");
                
                try 
                {
                    // Genero delle liste a parte per applicare i cambiamenti alla fine ed evitare errori di sincronizzazione durante i foreach
                    List<Territory> toRemove = new List<Territory>();
                    List<Territory> toAdd = new List<Territory>();

                    for(int i = 0; i <  GameManager.Instance.AllTerritories.Count; i++)
                    {
                        Territory terr = GameManager.Instance.AllTerritories[i];
                        //Debug.Log("[ALL_TERRITORIES] Territorio numero " + i);
                        //Debug.Log("Controllo il terr " + terr.name + " che appartiene a " + GameManager.Instance.getEnemyNameById(terr.player_id) + " che ha player_id = " + terr.player_id);
                        //Debug.Log("Per il meme, la grandezza di player.territories è: " + Player.Instance.Territories.Count);
                        Territory playerTerr = Player.Instance.Territories.Find(x => x.id == terr.id);
                            if (playerTerr is not null && terr.player_id != Player.Instance.PlayerId)
                            {
                               // Debug.Log("Il terr: " + terr.name + " è nella tua lista territori, ma appartiene a "
                                //          + GameManager.Instance.getEnemyNameById(terr.player_id));
                                toRemove.Add(playerTerr);
                            }
                            else if (playerTerr is null && terr.player_id == Player.Instance.PlayerId)
                            {
                             //   Debug.Log("Il terr: " + terr.name + " non è nella tua lista territori ma in realtà ti appartiene");
                                toAdd.Add(terr);
                            }
                            else if (playerTerr is null && terr.player_id != Player.Instance.PlayerId)
                            {
                                Debug.Log("Il territorio non è nella tua lista, non è nemmeno tuo, no work to do here");
                            }
                            else // Quando il territorio c'è nella mia lista ed è effettivamente mio
                            {
                                //Debug.Log("Aggiornati numero di tank del territorio " + Player.Instance.Territories.Find(x => x.id == terr.id).name);
                                //Debug.Log("Passati da " + Player.Instance.Territories.Find(x => x.id == terr.id).num_tanks + " Carri");
                                Player.Instance.Territories.Find(x => x.id == terr.id).num_tanks = terr.num_tanks;
                                //Debug.Log("ad adesso con " + Player.Instance.Territories.Find(x => x.id == terr.id).num_tanks + " carri!!!");
                            }
                    }
                    
                    Debug.Log("Sto per rimuovere " + toRemove.Count + " territori dalla mia lista, e sto per aggiungerne " + toAdd.Count);

                    // Applica le modifiche dopo aver completato l'iterazione
                    foreach (var terrToRemove in toRemove)
                    {
                        Player.Instance.Territories.Remove(terrToRemove);
                    }

                    foreach (var terrToAdd in toAdd)
                    {
                        Player.Instance.Territories.Add(terrToAdd);
                    }
                    Debug.Log("Done");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Errore: " + ex.Message);
                }

                GameManager.Instance.cleanAfterBattle();
                GameManager.Instance.setForceUpdateAfterAttack(true);
                Debug.Log("FORCED UPDATE FINISHED");
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
