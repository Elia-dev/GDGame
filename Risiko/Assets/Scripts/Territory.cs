public class Territory : Card
{
    public string name { get; set; }
    public int num_tanks { get; set; }
    public string continent { get; set; }
    public int node { get; set; }

    public Territory(string ter_id, string image, string function, string description, string playerId, string name, string continent, int node, int numTanks=1)
        : base(ter_id, image, function, description, playerId) {

        id = ter_id;
        this.name = name;
        num_tanks = numTanks;
        this.continent = continent;
        this.node = node;
    }

    public static Territory EmptyTerritory()
    {
        return new Territory(null, null, null, null, null, null, null, -1, -1);
    }

    public override string ToString()
    {
        return $"Territory(id={id}, image={image}, function={function}, description={description}, player_id={player_id}, name={name}, num_tanks={num_tanks}, continent={continent}, node={node})";
    }
}