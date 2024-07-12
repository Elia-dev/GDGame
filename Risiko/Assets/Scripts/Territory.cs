using System;
using System.Collections.Generic;

public class Territory : Card
{
    public string Name { get; set; }
    public int NumTanks { get; set; }
    public string Continent { get; set; }

    public Territory(string cardId, string image, string function, string description, string playerId, string name, int numTanks, string continent)
        : base(cardId, image, function, description, playerId)
    {
        Name = name;
        NumTanks = numTanks;
        Continent = continent;
    }

    public new Dictionary<string, object> ToDict()
    {
        var data = base.ToDict();
        data.Add("name", Name);
        data.Add("num_tanks", NumTanks);
        data.Add("continent", Continent);
        return data;
    }

    public static new Territory FromDict(Dictionary<string, object> data)
    {
        return new Territory(
            (string)data["id"],
            (string)data["image"],
            (string)data["function"],
            (string)data["description"],
            (string)data["player_id"],
            (string)data["name"],
            (int)data["num_tanks"],
            (string)data["continent"]
        );
    }

    public override string ToString()
    {
        return $"Territory(id={Id}, image={Image}, function={Function}, description={Description}, player_id={PlayerId}, name={Name}, num_tanks={NumTanks}, continent={Continent})";
    }
}