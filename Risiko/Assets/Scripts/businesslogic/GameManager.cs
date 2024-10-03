using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace businesslogic
{
    public class GameManager
    {
        private static GameManager _instance;
        private static readonly object Lock = new object();
        private Territory _enemyTerritory = null;
        private Territory _myTerritory = null;
        private readonly Dictionary<string, string> _playersDict = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _colorsDict = new Dictionary<string, string>();
        public List<Territory> AllTerritories = new List<Territory>();
        public readonly List<string> PlayersName = new List<string>();
        private readonly List<string> _availableColors = new List<string>();
        private string _winnerGameId = "";
        private string _winnerBattleId = "";
        private string _gameOrder = "";
        private int _extractedNumber = 0;
        private int[] _extractedEnemyNumbers = new int[3];
        private int[] _extractedMyNumbers = new int[3];
        private bool _gameWaitingToStart = true;
        private bool _gameRunning = true;
        private bool _gamePhase = false;
        private bool _imUnderAttack = false;
        private string _idPlayingPlayer = "";
        private int _enemyArmyNum = 0;
        private int _myArmyNum = 0;
        private bool _forceUpdateAfterAttack = false;
        private string _lobbyID = "";
        private bool _imAttacking = false;
        private string _killerID = "";
        private int _botNumber = 0;

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
    
        public void SetKillerId(string id)
        {
            _killerID = id;
        }
        
        public string GetKillerId()
        {
            return _killerID;
        }
        public void SetWinnerBattleId(string id)
        {
            _winnerBattleId = id;
        }

        public string GetWinnerBattleId()
        {
            return _winnerBattleId;
        }

        private void ResetWinnerBattleId()
        {
            _winnerBattleId = "";
        }
    
        public void SetWinnerGameId(string id)
        {
            _winnerGameId = id;
        }

        public string GetWinnerGameId()
        {
            return _winnerGameId;
        }
    
        public void SetEnemyExtractedNumbers(int[] vet)
        {
            _extractedEnemyNumbers = vet;
        }
    
        public int[] GetEnemyExtractedNumbers()
        {
            return _extractedEnemyNumbers;
        }

        private void ResetEnemyExtractedNumbers()
        {
            _extractedEnemyNumbers = null;
        }
    
        public void SetMyExtractedNumbers(int[] vet)
        {
            _extractedMyNumbers = vet;
        }

        public int[] GetMyExtractedNumbers()
        {
            return _extractedMyNumbers;
        }

        private void ResetMyExtractedNumbers()
        {
            _extractedMyNumbers = null;
        }
    
        public bool GetForceUpdateAfterAttack()
        {
            return _forceUpdateAfterAttack;
        }

        public void SetForceUpdateAfterAttack(bool value)
        {
            _forceUpdateAfterAttack = value;
        }

        public void CleanAfterBattle()
        {
            ResetEnemyArmyNum();
            ResetMyArmyNum();
            ResetEnemyTerritory();
            ResetMyTerritory();
            ResetEnemyExtractedNumbers();
            ResetMyExtractedNumbers();
            ResetWinnerBattleId();
            SetImUnderAttack(false);
            SetImAttacking(false);
        }
    
        public int GetMyArmyNum()
        {
            return _myArmyNum;
        }

        public void SetMyArmyNum(int numArmy)
        {
            _myArmyNum = numArmy;
        }

        private void ResetMyArmyNum()
        {
            _myArmyNum = 0;
        }
    
        public int GetEnemyArmyNum()
        {
            return _enemyArmyNum;
        }

        public void SetEnemyArmyNum(int numArmy)
        {
            _enemyArmyNum = numArmy;
        }

        private void ResetEnemyArmyNum()
        {
            _enemyArmyNum = 0;
        }
    
        public void AddPlayerColor(string id, string color)
        {
            _colorsDict.TryAdd(id, color);
        }
        public string GetPlayerColor(string id)
        {
            return _colorsDict.GetValueOrDefault(id, " ");
        }
    
        public string GetIdPlayingPlayer()
        {
            return _idPlayingPlayer;
        }

        public void SetIdPlayingPlayer(string player)
        {
            _idPlayingPlayer = player;
        }
    
        public string GetEnemyNameById(string playerId)
        {
            return _playersDict.GetValueOrDefault(playerId, " ");
        }
    
        public void AddPlayerToLobbyDict(string playerId, string name)
        {
            _playersDict.TryAdd(playerId, name);
        }
        public List<string> GetPlayersId()
        {
            return new List<string>(_playersDict.Keys);
        }
        public Territory GetMyTerritory()
        {
            if (_myTerritory is null)
            {
                _myTerritory = Territory.EmptyTerritory();
            }
            return _myTerritory;
        }

        public void SetMyTerritory(Territory ter)
        {
            _myTerritory = ter;
        }

        private void ResetMyTerritory()
        {
            _myTerritory = null;
        }
    
        public Territory GetEnemyTerritory()
        {
            if (_enemyTerritory is null)
            {
                _enemyTerritory = Territory.EmptyTerritory();
            }

            return _enemyTerritory;
        }

        private void ResetEnemyTerritory()
        {
            _enemyTerritory = null;
        }

        public void SetEnemyTerritoy(Territory ter)
        {
            _enemyTerritory = ter;
        }
        public void SetImUnderAttack(bool value)
        {
            _imUnderAttack = value;
        }

        public bool GetImUnderAttack()
        {
            return _imUnderAttack;
        }
        public void ResetGameManager()
        {
            _instance = null;
        }

        public void SetPreparationPhase(bool value)
        {
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
        public void SetGameOrderExtractedNumbers(string value)
        {
        }
    
        public string getGame_order()
        {
            // Regular expression to match "-0", "-1", etc.
            string pattern = @"-\d";
            return Regex.Replace(_gameOrder.Replace("{", "").Replace("'", "").Replace("}", ""), 
                pattern, string.Empty);
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
            _lobbyID = lobbyID;
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
            _availableColors.Add(color);
        }

        public List<string> GetAvailableColors()
        {
            return _availableColors;
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

        public void SetBotNumber(int num)
        {
            _botNumber = num;
        }

        public int GetBotNumber()
        {
            return _botNumber;
        }

        public void SetImAttacking(bool value)
        {
            _imAttacking = value;
        }
        public bool GetImAttacking()
        {
            return _imAttacking;
        }
    }
}
