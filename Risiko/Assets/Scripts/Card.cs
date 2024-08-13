using Newtonsoft.Json;

public class Card
{
    
    public string card_id { get; set; }
    public string Image { get; set; }
    public string Function { get; set; }
    public string Description { get; set; }
    public string player_id { get; set; }

    public Card(string cardId, string image, string function, string description, string playerId)
    {
        this.card_id = cardId;
        Image = image;
        Function = function;
        Description = description;
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
        return $"Card(id={card_id}, image={Image}, function={Function}, description={Description}, player_id={player_id})";
    }
}