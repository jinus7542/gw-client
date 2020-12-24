[System.Serializable]
public class Echo
{
    public string action;
    public string data;
}

[System.Serializable]
public class GameSessionInfo
{
    public string PlayerSessionId;
    public string PlayerId;
    public string GameSessionId;
    public string FleetId;
    public string CreationTime;
    public string Status;
    public string IpAddress;
    public int Port;
}

[System.Serializable]
public class MatchMakingRequestInfo
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerId;
    }

    public string TicketId;
    public string Status;
    public PlayerInfo[] Players;
}


[System.Serializable]
public class MatchStatusInfo
{
    public string IpAddress;
    public int Port;
    public string PlayerSessionId;
    public string DnsName;
}