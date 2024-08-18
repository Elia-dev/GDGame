using Newtonsoft.Json;

public class Card
{
    
    public string id { get; set; }
    public string image { get; set; }
    public string function { get; set; }
    public string description { get; set; }
    public string player_id { get; set; }

    public Card(string card_id, string image, string function, string description, string playerId)
    {
        id = card_id;
        this.image = image;
        this.function = function;
        this.description = description;
        player_id = playerId;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public static Card FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Card>(json);
    }
    
    public override string ToString()
    {
        return $"Card(id={id}, image={image}, function={function}, description={description}, player_id={player_id})";
    }
}