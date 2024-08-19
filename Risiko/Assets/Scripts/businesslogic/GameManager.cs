using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager
{
    private static GameManager _instance;
    private static readonly object Lock = new object();
    private Territory _enemyAttackerTerritory = null;
    private Dictionary<string, string> _playersDict = new Dictionary<string, string>();
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
    private string _playingPlayer = "";

    private string _lobbyID;
    /*
     * - Stato  fantoccio da riempire con le richieste mandate al server.
     * "esempio" seleziono il canada, quindi chiedo al server info sul canada, oil server mi manda le info che io vado a mettere
     * nello stato fantoccio
     *
     * - Dizionario idPlayer - Nome
     * - idLobby
     * - 
     */
    
    
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

    public string getPlayingPlayer()
    {
        return _playingPlayer;
    }

    public void setPlayingPlayer(string player)
    {
        _playingPlayer = player;
    }
    
    public string getEnemyNameById(string playerId)
    {
        if (_playersDict.TryGetValue(playerId, out string name))
        {
            Debug.Log($"Player trovato: ID = {playerId}, Nome = {name}");
            return name;
        }
        Debug.Log($"Player non trovato: ID = {playerId}");
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
}
