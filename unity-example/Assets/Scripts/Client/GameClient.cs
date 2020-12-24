using System;
using System.Threading.Tasks;
using UnityEngine;
using LitJson;

#if CLIENT

public class GameClient
{
    // Not used in the example but can be used to request game session directly from the fleet without matchmaking
    public GameSessionInfo GameSession()
    {
        try
        {
            var query = $"gamesession" +
                        $"{{ PlayerSessionId, PlayerId, GameSessionId, FleetId, CreationTime, Status, IpAddress, Port }}";
            var data = Task.Run(() => GraphClient.Mutation(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                var error = JsonMapper.ToObject(data.Result[1]);
                Debug.Log((string)error["extensions"]["code"]);
                return null;
            }
            return JsonMapper.ToObject<GameSessionInfo>(data.Result[1]);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    // Sends a new matchmaking request ticket to the backend API
    public MatchMakingRequestInfo MatchMaking()
    {
        try
        {
            var query = $"matchmaking" +
                        $"{{ TicketId, Status, Players {{ PlayerId }} }}";
            var data = Task.Run(() => GraphClient.Mutation(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                var error = JsonMapper.ToObject(data.Result[1]);
                Debug.Log((string)error["extensions"]["code"]);
                return null;
            }
            return JsonMapper.ToObject<MatchMakingRequestInfo>(data.Result[1]);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    // Checks the status of a matchmaking request ticket
    public MatchStatusInfo MatchStatus(string ticketId)
    {
        try
        {
            var query = $"matchstatus(ticketId: \"{ticketId}\")" +
                        $"{{ IpAddress, Port, PlayerSessionId, DnsName }}";
            var data = Task.Run(() => GraphClient.Query(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                var error = JsonMapper.ToObject(data.Result[1]);
                Debug.Log((string)error["extensions"]["code"]);
                return null;
            }
            return JsonMapper.ToObject<MatchStatusInfo>(data.Result[1]);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }
}

#endif
