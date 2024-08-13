using Newtonsoft.Json;

public class Territory : Card
{
    public string Name { get; set; }
    public int NumTanks { get; set; }
    public string Continent { get; set; }

    public Territory(string id, string image, string function, string description, string playerId, string name, int numTanks, string continent)
        : base(id, image, function, description, playerId) {

        base.id = id;
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
        return $"Territory(id={id}, image={Image}, function={Function}, description={Description}, player_id={player_id}, name={Name}, num_tanks={NumTanks}, continent={Continent})";
    }
}