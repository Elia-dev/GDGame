using Newtonsoft.Json;

public class Card
{
    
    public string CardId { get; set; }
    public string Image { get; set; }
    public string Function { get; set; }
    public string Description { get; set; }
    public string PlayerId { get; set; }

    public Card(string cardCardId, string image, string function, string description, string playerId)
    {
        CardId = cardCardId;
        Image = image;
        Function = function;
        Description = description;
        PlayerId = playerId;
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
        return $"Card(id={CardId}, image={Image}, function={Function}, description={Description}, player_id={PlayerId})";
    }
}