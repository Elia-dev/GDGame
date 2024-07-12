using System;
using System.Collections.Generic;
using System.Linq;

public class Player
{
    public string Name { get; set; }
    public object Sock { get; set; } // Assumendo che il tipo di socket sia object per semplicità
    public string LobbyId { get; set; }
    public string PlayerId { get; set; }
    public int TanksNum { get; set; }
    public int TanksAvailable { get; set; }
    public int TanksPlaced { get; set; }
    public Objective ObjectiveCard { get; set; }
    public List<Territory> Territories { get; set; } = new List<Territory>();

    public Player(object socket, string name, string lobbyId, string playerId)
    {
        Name = name;
        Sock = socket;
        LobbyId = lobbyId;
        PlayerId = playerId;
        TanksNum = 0;
        TanksAvailable = 0;
        TanksPlaced = 0;
        ObjectiveCard = null;
        Territories = new List<Territory>();
    }

    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            { "name", Name },
            { "sock", "sock" }, // Come rappresentare il socket?
            { "lobby_id", LobbyId },
            { "player_id", PlayerId },
            { "tanks_num", TanksNum },
            { "tanks_available", TanksAvailable },
            { "tanks_placed", TanksPlaced },
            { "objective_card", ObjectiveCard?.ToDict() },
            { "territories", Territories.Select(t => t.ToDict()).ToList() }
        };
    }

    public override string ToString()
    {
        return $"Player(name={Name}, socket={Sock}, lobby_id={LobbyId}, player_id={PlayerId}, tanks_num={TanksNum}, tanks_available={TanksAvailable}, tanks_placed={TanksPlaced}, objective_card={ObjectiveCard}, territories={Territories})";
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
        var territory = Territories.FirstOrDefault(t => t.Id == territoryId);
        if (territory != null)
        {
            territory.NumTanks += 1;
        }
    }

    public static Player FromDict(Dictionary<string, object> data)
    {
        var player = new Player(data["sock"], (string)data["name"], (string)data["lobby_id"], (string)data["player_id"]);
        player.TanksNum = (int)data["tanks_num"];
        player.TanksAvailable = (int)data["tanks_available"];
        player.TanksPlaced = (int)data["tanks_placed"];
        player.ObjectiveCard = Objective.FromDict((Dictionary<string, object>)data["objective_card"]);
        player.Territories = ((List<object>)data["territories"]).Select(t => Territory.FromDict((Dictionary<string, object>)t)).ToList();
        return player;
    }
}
