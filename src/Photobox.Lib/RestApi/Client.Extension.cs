using System.Net.Http.Headers;
using MethodTimer;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.Helper;

namespace Photobox.Lib.RestApi;

public partial class ImageClient
{
    //TODO implement proper injection of the IAccessTokenManager

    public static IAccessTokenManager AccessTokenManager;

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        string? accessToken = AccessTokenManager.AccessToken.Result;

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add(PhotoboxHeaders.HardwareId, PhotoboxHelper.PhotoboxId);
        }
    }
}

public partial class PhotoBoxClient
{
    //TODO implement proper injection of the IAccessTokenManager

    public static IAccessTokenManager AccessTokenManager;

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        string? accessToken = AccessTokenManager.AccessToken.Result;

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add(PhotoboxHeaders.HardwareId, PhotoboxHelper.PhotoboxId);
        }
    }
}
