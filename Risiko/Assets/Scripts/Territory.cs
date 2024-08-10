using Newtonsoft.Json;

public class Territory : Card
{
    public string Name { get; set; }
    public int NumTanks { get; set; }
    public string Continent { get; set; }

    public Territory(string cardCardId, string image, string function, string description, string playerId, string name, int numTanks, string continent)
        : base(cardCardId, image, function, description, playerId) {

        CardId = cardCardId;
        Name = name;
        NumTanks = numTanks;
        Continent = continent;
    }
    
    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public new static Territory FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Territory>(json);
    }

    public override string ToString()
    {
        return $"Territory(id={CardId}, image={Image}, function={Function}, description={Description}, player_id={PlayerId}, name={Name}, num_tanks={NumTanks}, continent={Continent})";
    }
}