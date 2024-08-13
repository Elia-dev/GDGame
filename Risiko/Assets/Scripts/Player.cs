using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class Player
{

	private static Player _instance = null;
    private static readonly object _lock = new object();

	private Player() { }
    
    private Player(object socket, string name, string lobbyId, string playerId)
    {
            Sock = socket;
            Name = name;
            LobbyId = lobbyId; // Forse questo lo possiamo togliere, sul player non serve perché è già salvato in ClientManager
            PlayerId = playerId;
            TanksNum = 0;
            TanksAvailable = 0;
            TanksPlaced = 0;
            ObjectiveCard = null;
            Territories = new List<Territory>();
            IsMyTurn = false;
            ArmyColor = null;
    }

    public string ArmyColor { get; set; }

    public void Initialize(object socket, string name, string lobbyId, string playerId)
    {
        if (Sock == null && Name == null && LobbyId == null && PlayerId == null)
        {
            Sock = socket;
            Name = name;
            LobbyId = lobbyId;
            PlayerId = playerId;
            TanksNum = 0;
            TanksAvailable = 0;
            TanksPlaced = 0;
            ObjectiveCard = null;
            Territories = new List<Territory>();
        }
    }

    public void Initialize()
    {
        PlayerId = null;
    }
    
    public static Player Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
                return _instance;
            }
        }
    }


    public bool IsMyTurn { get; set; }
    public string Name { get; set; }
    public object Sock { get; set; } // Assumendo che il tipo di socket sia object per semplicità
    public string LobbyId { get; set; }
    public string PlayerId { get; set; }
    public int TanksNum { get; set; }
    public int TanksAvailable { get; set; }
    public int TanksPlaced { get; set; }
    public Objective ObjectiveCard { get; set; }
    public List<Territory> Territories { get; set; } = new List<Territory>();

    public override string ToString()
    {
        return $"Player(name={Name}, socket={Sock}, lobby_id={LobbyId}, player_id={PlayerId}, tanks_num={TanksNum}, tanks_available={TanksAvailable}, tanks_placed={TanksPlaced}, objective_card={ObjectiveCard}, territories={Territories})";
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public new static Player FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Player>(json);
    }
    
    public void SetObjectiveCard(Objective goalCard)
    {
        ObjectiveCard = goalCard;
    }

    public void AddTerritory(Territory territoryCard)
    {
        Territories.Add(territoryCard);
    }

    public void RemoveTerritory(Territory territoryCard)
    {
        Territories.Remove(territoryCard);
    }

    public List<Territory> GetTerritories()
    {
        return Territories;
    }

    public void AddTankToTerritory(string territoryId)
    {
        TanksAvailable -= 1;
        TanksPlaced += 1;
        var territory = Territories.FirstOrDefault(t => t.id == territoryId);
        if (territory != null)
        {
            territory.NumTanks += 1;
        }
    }
}
