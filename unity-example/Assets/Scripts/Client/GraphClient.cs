using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

#if CLIENT

public class GraphClient
{
    [System.Serializable]
    class gql
    {
        public string query;
    }

    public static async Task<string> PostAsync(string requestUri, object content, double timeoutSeconds = 10)
    {
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            using (var stringContent = new StringContent(JsonMapper.ToJson(content), Encoding.UTF8, "application/json"))
            {
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                request.Content = stringContent;
                using (var response = await client.PostAsync(
                    requestUri,
                    stringContent,
                    regionName: Global.regionString,
                    serviceName: "execute-api",
                    credentials: Client.cognitoCredentials))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }

    public static List<string> ToJsons(string response)
    {
        //Debug.Log(response);
        var root = JsonMapper.ToObject(response);
        if (!root.Keys.Contains("errors") && !root.Keys.Contains("data"))
        {
            throw new Exception(response);
        }
        var j0 = (root.Keys.Contains("errors")) ? root["errors"] : root["data"];
        var data = new List<string> { (root.Keys.Contains("errors")) ? "errors" : "data" };
        for (int i = 0; i < j0.Count; i++)
        {
            data.Add(JsonMapper.ToJson(j0[i]));
        }
        return data;
    }

    public static async Task<List<string>> Query(string requestUri, string query, double timeoutSeconds = 10)
    {
        var content = new gql { query = $"query {{ {query} }}" };
        //Debug.Log(content.query);
        var response = await GraphClient.PostAsync(requestUri, content, timeoutSeconds);
        return GraphClient.ToJsons(response);
    }

    public static async Task<List<string>> Mutation(string requestUri, string query, double timeoutSeconds = 10)
    {
        var content = new gql { query = $"mutation {{ {query} }}" };
        //Debug.Log(content.query);
        var response = await GraphClient.PostAsync(requestUri, content, timeoutSeconds);
        return GraphClient.ToJsons(response);
    }
}

#endif
