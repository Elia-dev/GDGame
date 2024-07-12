public class Card
{
    public int Id { get; set; }
    public string Image { get; set; }
    public string Function { get; set; }
    public string Description { get; set; }
    public int PlayerId { get; set; }

    public Card(int cardId, string image, string function, string description, int playerId)
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
            (int)data["id"],
            (string)data["image"],
            (string)data["function"],
            (string)data["description"],
            (int)data["player_id"]
        );
    }

    public override string ToString()
    {
        return $"Card(id={Id}, image={Image}, function={Function}, description={Description}, player_id={PlayerId})";
    }
}