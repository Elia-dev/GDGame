using System.Collections.Generic;

public class Player
{

	private static Player _instance = null;
    private static readonly object Lock = new object();

	private Player() { }

    public string ArmyColor { get; set; }

    public void Initialize()
    {
        PlayerId = null;
    }
    
    public static Player Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
                return _instance;
            }
        }
    }

    public void ResetPlayer()
    {
        _instance = null;
    }
    
    public bool IsMyTurn { get; set; }
    public string Name { get; set; }
    public object Sock { get; set; }
    public string PlayerId { get; set; }
    public int TanksNum { get; set; }
    public int TanksAvailable { get; set; }
    public int TanksPlaced { get; set; }
    public Objective ObjectiveCard { get; set; }
    public List<Territory> Territories { get; set; } = new List<Territory>();
}
