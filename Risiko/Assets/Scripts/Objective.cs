using System;
using System.Collections.Generic;

public class Objective : Card {
    private string cardId;

    public string CardId {
        get => cardId;
        set => cardId = value;
    }

    private string image { get; set; }
    private string function { get; set; }
    private string description { get; set; }
    private string playerId { get; set; }
    public Objective(string cardId, string image, string function, string description, string playerId)
        : base(cardId, image, function, description, playerId)
    {
    }

    public new Dictionary<string, object> ToDict()
    {
        var data = base.ToDict();
        return data;
    }

    public static new Objective FromDict(Dictionary<string, object> data)
    {
        return new Objective(
            (string)data["id"],
            (string)data["image"],
            (string)data["function"],
            (string)data["description"],
            (string)data["player_id"]
        );
    }
}
