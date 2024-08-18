using Newtonsoft.Json;

public class Territory : Card
{
    public string Name { get; set; }
    public int num_tanks { get; set; }
    public string Continent { get; set; }
    
    public int node { get; set; }

    public Territory(string ter_id, string image, string function, string description, string playerId, string name, string continent, int node, int numTanks=1)
        : base(ter_id, image, function, description, playerId) {

        id = ter_id;
        Name = name;
        num_tanks = numTanks;
        Continent = continent;
        this.node = node;
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
        return $"Territory(id={id}, image={Image}, function={Function}, description={Description}, player_id={player_id}, name={Name}, num_tanks={num_tanks}, continent={Continent}, node={node})";
    }
}