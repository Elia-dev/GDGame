public class Lobby
{
    private readonly string _lobbyID;
    private readonly string _hostName;
    private readonly int _playersNum;

    public Lobby(string lobbyId, string hostName, int playersNum)
    {
        _lobbyID = lobbyId;
        _hostName = hostName;
        _playersNum = playersNum;
    }

    public string GetLobbyID()
    {
        return _lobbyID;
    }

    public string GetHostName()
    {
        return _hostName;
    }

    public int GetPlayersNum()
    {
        return _playersNum;
    }
    
}
