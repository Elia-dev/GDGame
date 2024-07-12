using System.Collections.Generic;

public class Card
{
    
    public string Id { get; set; }
    public string Image { get; set; }
    public string Function { get; set; }
    public string Description { get; set; }
    public string PlayerId { get; set; }

    public Card(string cardId, string image, string function, string description, string playerId)
    {
        Id = cardId;
        Image = image;
        Function = function;
        Description = description;
        PlayerId = playerId;
    }

    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            { "id", Id },
            { "image", Image },
            { "function", Function },
            { "description", Description },
            { "player_id", PlayerId }
        };
    }

    public static Card FromDict(Dictionary<string, object> data)
    {
        return new Card(
            (string)data["id"],
            (string)data["image"],
            (string)data["function"],
            (string)data["description"],
            (string)data["player_id"]
        );
    }

    public override string ToString()
    {
        return $"Card(id={Id}, image={Image}, function={Function}, description={Description}, player_id={PlayerId})";
    }
}