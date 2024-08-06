using Newtonsoft.Json;

public class Objective : Card {
    public Objective(string cardCardId, string image, string function, string description, string playerId)
        : base(cardCardId, image, function, description, playerId)
    {
    }
    
    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public new static Objective FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Objective>(json);
    }
    
}
