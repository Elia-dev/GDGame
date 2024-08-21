using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager
{
    private static GameManager _instance;
    private static readonly object Lock = new object();
    private Territory _enemyAttackerTerritory = null;
    private Territory _myTerritoryUnderAttack = null;
    private Dictionary<string, string> _playersDict = new Dictionary<string, string>();
    private Dictionary<string, string> _colorsDict = new Dictionary<string, string>();
    public List<Territory> AllTerritories = new List<Territory>(); // Lista di tutti i territori della partita
    public List<string> PlayersName = new List<string>(); 
    public List<string> AvailableColors = new List<string>(); 
    private string _gameOrder = "";
    private int _extractedNumber = 0;
    private string _gameOrderExtractedNumbers = "";
    private bool _gameWaitingToStart = true;
    private bool _gameRunning = true;
    private bool _preparationPhase = true;
    private bool _gamePhase = false;
    private bool _imUnderAttack = false;
    private string _idPlayingPlayer = "";
    private int _enemyAttackerArmyNum = 0;
    private int _myArmyNumToDefende = 0;

    private string _lobbyID;
    private bool _imAttacking = false;

    private GameManager() // Private constructor to allow instantiation using singleton only
    {
    }
    public static GameManager Instance // Implementing singleton pattern
    {
        get
        {
            // Using lock to manage concurrency
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }
    }

    public int getMyArmyNumToDefende()
    {
        return _myArmyNumToDefende;
    }

    public void setMyArmyNumToDefende(int numArmy)
    {
        _myArmyNumToDefende = numArmy;
    }

    public void resetMyArmyNumToDefende()
    {
        _myArmyNumToDefende = 0;
    }
    
    public int GetEnemyAttackerArmyNum()
    {
        return _enemyAttackerArmyNum;
    }

    public void setEnemyAttackerArmyNum(int numArmy)
    {
        _enemyAttackerArmyNum = numArmy;
    }

    public void resetEnemyAttackerArmyNum()
    {
        _enemyAttackerArmyNum = 0;
    }
    
    public void AddPlayerColor(string id, string color)
    {
        if (!_colorsDict.ContainsKey(id))
        {
            _colorsDict.Add(id, color);
            Debug.Log($"Colore aggiunto: ID = {id}, Colore = {color}");
        }
        else
        {
            Debug.Log($"Il colore con ID = {id} esiste già.");
        }
    }

    // Funzione per rimuovere un colore dal dizionario
    public void RemovePlayerColor(string id)
    {
        if (_colorsDict.ContainsKey(id))
        {
            _colorsDict.Remove(id);
            Debug.Log($"Colore con ID = {id} rimosso.");
        }
        else
        {
            Debug.Log($"Il colore con ID = {id} non esiste.");
        }
    }

    // Funzione per leggere un colore dal dizionario
    public string GetPlayerColor(string id)
    {
        if (_colorsDict.TryGetValue(id, out string color))
        {
            Debug.Log($"Colore trovato: ID = {id}, Colore = {color}");
            return color;
        }
        Debug.Log($"Il colore con ID = {id} non è stato trovato.");
        return "Player non trovato";
    }
    
    public string getIdPlayingPlayer()
    {
        return _idPlayingPlayer;
    }

    public void setIdPlayingPlayer(string player)
    {
        _idPlayingPlayer = player;
    }

    public void resetIdPlayingPlayer()
    {
        _idPlayingPlayer = "";
    }
    
    public string getEnemyNameById(string playerId)
    {
        if (_playersDict.TryGetValue(playerId, out string name))
        {
            //Debug.Log($"Player trovato: ID = {playerId}, Nome = {name}");
            return name;
        }
        //Debug.Log($"Player non trovato: ID = {playerId}");
        return "This player doesn't exist";
    }
    
    public void AddPlayerToLobbyDict(string playerId, string name)
    {
        if (!_playersDict.ContainsKey(playerId))
        {
            _playersDict.Add(playerId, name);
            Debug.Log($"Player aggiunto: ID = {playerId}, Nome = {name}");
        }
        else
        {
            Debug.Log($"Il player con ID = {playerId} esiste già.");
        }
    }

    // Funzione per rimuovere un player dal dizionario
    public void RemovePlayerFromLobbyDict(string playerId)
    {
        if (_playersDict.ContainsKey(playerId))
        {
            _playersDict.Remove(playerId);
            Debug.Log($"Player con ID = {playerId} rimosso.");
        }
        else
        {
            Debug.Log($"Il player con ID = {playerId} non esiste.");
        }
    }
    
    public Territory getMyTerritoryUnderAttack()
    {
        if (_myTerritoryUnderAttack is null)
        {
            _myTerritoryUnderAttack = Territory.EmptyTerritory();
        }

        return _myTerritoryUnderAttack;
    }
    
    public void deleteMyTerritoryUnderAttack()
    {
        _myTerritoryUnderAttack = null;
    }
    
    public Territory getEnemyAttackerTerritory()
    {
        if (_enemyAttackerTerritory is null)
        {
            _enemyAttackerTerritory = Territory.EmptyTerritory();
        }

        return _enemyAttackerTerritory;
    }

    public void deleteAttackerTerritory()
    {
        _enemyAttackerTerritory = null;
    }
    public void setImUnderAttack(bool value)
    {
        _imUnderAttack = value;
    }

    public bool getImUnderAttack()
    {
        return _imUnderAttack;
    }
    public void ResetGameManager()
    {
        _instance = null;
    }
    
    public bool GetPreparationPhase()
    {
        return _preparationPhase;
    }

    public void SetPreparationPhase(bool value)
    {
        _preparationPhase = value;
    }

    public bool GetGamePhase()
    {
        return _gamePhase;
    }

    public void SetGamePhase(bool value)
    {
        _gamePhase = value;
    }
    public int GetExtractedNumber()
    {
        return _extractedNumber;
    }

    public void SetExtractedNumber(int value)
    {
        _extractedNumber = value;
    }

    public string GetGameOrderExtractedNumbers()
    {
        return _gameOrderExtractedNumbers;
    }
    public void SetGameOrderExtractedNumbers(string value)
    {
        _gameOrderExtractedNumbers = value;
    }
    
    public string getGame_order()
    {
        return _gameOrder;
    }
    public void setGame_order(string value)
    {
        _gameOrder = value;
    }
    
    public string GetLobbyId()
    {
        return _lobbyID;
    }
    public void SetLobbyId(string lobbyID)
    {
        this._lobbyID = lobbyID;
    }

    public void ResetPlayersName()
    {
        PlayersName.Clear();
    }

    public void AddPlayerName(string name)
    {
        PlayersName.Add(name);
    }

    public int GetPlayersNumber()
    {
        return PlayersName.Count;
    }

    public void AddAvailableColor(string color)
    {
        AvailableColors.Add(color);
    }

    public List<string> GetAvailableColors()
    {
        return AvailableColors;
    }
    
    public bool GetGameWaitingToStart()
    {
        return _gameWaitingToStart;
    }

    public void SetGameWaitingToStart(bool value)
    {
        _gameWaitingToStart = value;
    }
    
    public bool GetGameRunning()
    {
        return _gameRunning;
    }

    public void SetGameRunning(bool value)
    {
        _gameRunning = value;
    }

    public void setImAttacking(bool value)
    {
        _imAttacking = true;
    }
    public bool getImAttacking()
    {
        return _imAttacking;git
    }
}
