using Photobox.Lib.AccessTokenManager;
using System.Net.Http.Headers;

namespace Photobox.Lib.RestApi;

public partial class Client
{
    async partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        string? accessToken = "";//await accessTokenManager.AccessToken;
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}