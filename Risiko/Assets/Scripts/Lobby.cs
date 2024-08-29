using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby
{
    private string _lobbyID;
    private string _hostName;
    private int _playersNum;

    public Lobby(string lobbyId, string hostName, int playersNum)
    {
        _lobbyID = lobbyId;
        _hostName = hostName;
        _playersNum = playersNum;
    }

    public void setLobbyID(string lobbyID)
    {
        _lobbyID = lobbyID;
    }

    public string getLobbyID()
    {
        return _lobbyID;
    }

    public void setHostName(string hostName)
    {
        _hostName = hostName;
    }

    public string getHostName()
    {
        return _hostName;
    }

    public void setPlayersNum(int playersNum)
    {
        _playersNum = playersNum;
    }

    public int getPlayersNum()
    {
        return _playersNum;
    }
    
}
