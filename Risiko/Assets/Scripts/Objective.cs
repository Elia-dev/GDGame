using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

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
    
    
    
    public static Objective FromJson(string jsonData)
    {
        Objective objective;
        using (StringReader stringReader = new StringReader(jsonData))
        {
            // Creare un JsonTextReader per leggere dal StringReader
            using (JsonTextReader jsonReader = new JsonTextReader(stringReader))
            {
                // Deserializzare il JSON in un oggetto Card
                JsonSerializer serializer = new JsonSerializer();
                objective = serializer.Deserialize<Objective>(jsonReader);

                // Utilizzare l'oggetto deserializzato
                Console.WriteLine($"Id: {objective.Id}, Image: {objective.Image}");
            }
        }

        return objective;
        
        // Crea e ritorna un'istanza di Objective utilizzando i dati deserializzati
        /*
        return new Objective(
            data["id"].ToString(),
            data["image"].ToString(),
            data["function"].ToString(),
            data["description"].ToString(),
            data["player_id"].ToString()
        );
        */
    }
}

public class JsonElement
{
}
