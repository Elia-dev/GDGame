using Newtonsoft.Json;

public class Objective : Card {
    public Objective(string card_id, string image, string function, string description, string player_id)
        : base(card_id, image, function, description, player_id)
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
