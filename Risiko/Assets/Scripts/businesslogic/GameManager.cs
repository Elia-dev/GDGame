using System.Collections.Generic;
using UnityEngine;

namespace businesslogic
{
    public class GameManager
    {
        private static GameManager _instance;
        private static readonly object Lock = new object();
        private Territory _enemyTerritory = null;
        private Territory _myTerritory = null;
        private Dictionary<string, string> _playersDict = new Dictionary<string, string>();
        private Dictionary<string, string> _colorsDict = new Dictionary<string, string>();
        public List<Territory> AllTerritories = new List<Territory>(); // Lista di tutti i territori della partita
        public List<string> PlayersName = new List<string>(); 
        public List<string> AvailableColors = new List<string>();
        private string _winnerGameId = "";
        private string _winnerBattleId = "";
        private string _gameOrder = "";
        private int _extractedNumber = 0;
        private int[] _extractedEnemyNumbers = new int[3];
        private int[] _extractedMyNumbers = new int[3];
        private string _gameOrderExtractedNumbers = "";
        private bool _gameWaitingToStart = true;
        private bool _gameRunning = true;
        private bool _preparationPhase = true;
        private bool _gamePhase = false;
        private bool _imUnderAttack = false;
        private string _idPlayingPlayer = "";
        private int _enemyArmyNum = 0;
        private int _myArmyNum = 0;
        private bool _forceUpdateAfterAttack = false;
        private string _lobbyID = "";
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

        public void setWinnerBattleId(string id)
        {
            _winnerBattleId = id;
        }

        public string getWinnerBattleId()
        {
            return _winnerBattleId;
        }

        public void resetWinnerBattleId()
        {
            _winnerBattleId = "";
        }
    
        public void setWinnerGameId(string id)
        {
            _winnerGameId = id;
        }

        public string getWinnerGameId()
        {
            return _winnerGameId;
        }
    
        public void setEnemyExtractedNumbers(int[] vet)
        {
            _extractedEnemyNumbers = vet;
        }
    
        public int[] getEnemyExtractedNumbers()
        {
            return _extractedEnemyNumbers;
        }
    
        public void resetEnemyExtractedNumbers()
        {
            _extractedEnemyNumbers = null;
        }
    
        public void setMyExtractedNumbers(int[] vet)
        {
            _extractedMyNumbers = vet;
        }

        public int[] getMyExtractedNumbers()
        {
            return _extractedMyNumbers;
        }
    
        public void resetMyExtractedNumbers()
        {
            _extractedMyNumbers = null;
        }
    
        public bool getForceUpdateAfterAttack()
        {
            return _forceUpdateAfterAttack;
        }

        public void setForceUpdateAfterAttack(bool value)
        {
            _forceUpdateAfterAttack = value;
        }

        public void cleanAfterBattle()
        {
            resetEnemyArmyNum();
            resetMyArmyNum();
            resetEnemyTerritory();
            resetMyTerritory();
            resetEnemyExtractedNumbers();
            resetMyExtractedNumbers();
            resetWinnerBattleId();
            setImUnderAttack(false);
            setImAttacking(false);
        }
    
        public int getMyArmyNum()
        {
            return _myArmyNum;
        }

        public void setMyArmyNum(int numArmy)
        {
            _myArmyNum = numArmy;
        }

        public void resetMyArmyNum()
        {
            _myArmyNum = 0;
        }
    
        public int GetEnemyArmyNum()
        {
            return _enemyArmyNum;
        }

        public void setEnemyArmyNum(int numArmy)
        {
            _enemyArmyNum = numArmy;
        }

        public void resetEnemyArmyNum()
        {
            _enemyArmyNum = 0;
        }
    
        public void AddPlayerColor(string id, string color)
        {
            if (!_colorsDict.ContainsKey(id))
            {
                _colorsDict.Add(id, color);
                //Debug.Log($"Colore aggiunto: ID = {id}, Colore = {color}");
            }
            else
            {
                //Debug.Log($"Il colore con ID = {id} esiste già.");
            }
        }

        // Funzione per rimuovere un colore dal dizionario
        public void RemovePlayerColor(string id)
        {
            if (_colorsDict.ContainsKey(id))
            {
                _colorsDict.Remove(id);
                //Debug.Log($"Colore con ID = {id} rimosso.");
            }
            else
            {
                //Debug.Log($"Il colore con ID = {id} non esiste.");
            }
        }

        // Funzione per leggere un colore dal dizionario
        public string GetPlayerColor(string id)
        {
            if (_colorsDict.TryGetValue(id, out string color))
            {
                //Debug.Log($"Colore trovato: ID = {id}, Colore = {color}");
                return color;
            }
            //Debug.Log($"Il colore con ID = {id} non è stato trovato.");
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
                //Debug.Log($"Player aggiunto: ID = {playerId}, Nome = {name}");
            }
            else
            {
                //Debug.Log($"Il player con ID = {playerId} esiste già.");
            }
        }

        public List<string> GetPlayersName()
        {
            return PlayersName;
        }
        
        public List<string> GetPlayersId()
        {
            return new List<string>(_playersDict.Keys);
        }
        
        // Funzione per rimuovere un player dal dizionario
        public void RemovePlayerFromLobbyDict(string playerId)
        {
            if (_playersDict.ContainsKey(playerId))
            {
                _playersDict.Remove(playerId);
                //Debug.Log($"Player con ID = {playerId} rimosso.");
            }
            else
            {
                //Debug.Log($"Il player con ID = {playerId} non esiste.");
            }
        }
    
        public Territory getMyTerritory()
        {
            if (_myTerritory is null)
            {
                _myTerritory = Territory.EmptyTerritory();
            }

            return _myTerritory;
        }

        public void setMyTerritory(Territory ter)
        {
            _myTerritory = ter;
        }
    
        public void resetMyTerritory()
        {
            _myTerritory = null;
        }
    
        public Territory getEnemyTerritory()
        {
            if (_enemyTerritory is null)
            {
                _enemyTerritory = Territory.EmptyTerritory();
            }

            return _enemyTerritory;
        }

        public void resetEnemyTerritory()
        {
            _enemyTerritory = null;
        }

        public void setEnemyTerritoy(Territory ter)
        {
            _enemyTerritory = ter;
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
            _imAttacking = value;
        }
        public bool getImAttacking()
        {
            return _imAttacking;
        }
    }
}
