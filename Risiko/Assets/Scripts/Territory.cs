using Newtonsoft.Json;

public class Territory : Card
{
    public string Name { get; set; }
    public int NumTanks { get; set; }
    public string Continent { get; set; }

    public Territory(string cardId, string image, string function, string description, string playerId, string name, int numTanks, string continent)
        : base(cardId, image, function, description, playerId) {

        base.card_id = cardId;
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
        return $"Territory(id={card_id}, image={Image}, function={Function}, description={Description}, player_id={player_id}, name={Name}, num_tanks={NumTanks}, continent={Continent})";
    }
}