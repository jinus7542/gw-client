using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using LitJson;

#if CLIENT

public class RestClient
{
    public static async Task<string> PostAsync(string requestUri, object content = null, double timeoutSeconds = 10)
    {
        var credentials = new ImmutableCredentials(Client.cognitoCredentials.AccessKey, Client.cognitoCredentials.SecretKey, Client.cognitoCredentials.Token);
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
                    credentials: credentials))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }

    public static async Task<string> GetAsync(string requestUri, double timeoutSeconds = 10)
    {
        var credentials = new ImmutableCredentials(Client.cognitoCredentials.AccessKey, Client.cognitoCredentials.SecretKey, Client.cognitoCredentials.Token);
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            using (var response = await client.GetAsync(
                requestUri,
                regionName: Global.regionString,
                serviceName: "execute-api",
                credentials: credentials))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    public static async Task<string> PutAsync(string requestUri, object content = null, double timeoutSeconds = 10)
    {
        var credentials = new ImmutableCredentials(Client.cognitoCredentials.AccessKey, Client.cognitoCredentials.SecretKey, Client.cognitoCredentials.Token);
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            using (var stringContent = new StringContent(JsonMapper.ToJson(content), Encoding.UTF8, "application/json"))
            {
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                request.Content = stringContent;
                using (var response = await client.PutAsync(
                    requestUri,
                    stringContent,
                    regionName: Global.regionString,
                    serviceName: "execute-api",
                    credentials: credentials))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }

    public static async Task<string> DeleteAsync(string requestUri, double timeoutSeconds = 10)
    {
        var credentials = new ImmutableCredentials(Client.cognitoCredentials.AccessKey, Client.cognitoCredentials.SecretKey, Client.cognitoCredentials.Token);
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            using (var response = await client.DeleteAsync(
                requestUri,
                regionName: Global.regionString,
                serviceName: "execute-api",
                credentials: credentials))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

#endif
