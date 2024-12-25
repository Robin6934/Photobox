using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.Options;
using Photobox.Lib.RestApi;
using Photobox.UI.Lib.ConfigModels;
using Timer = System.Timers.Timer;

namespace Photobox.UI.Lib.CredentialManager;

public class RefreshTokenManager(IOptionsMonitor<PhotoboxConfig> configMonitor, IClient photoBoxClient) : IRefreshTokenManager
{
    private readonly Timer _refreshTimer = new();

    public async Task LoginAsync(string email, string password)
    {
        var loginRequest = new LoginRequest()
        {
            Email = email, 
            Password = password
        };

        var loginResponse = await photoBoxClient.PostLoginAsync(false, false, loginRequest);



    }

    public bool LoggedIn => string.IsNullOrEmpty(configMonitor.CurrentValue.RefreshToken.Value);

    public string RetrieveToken()
    {
        return configMonitor.CurrentValue.RefreshToken.Value;
    }

    private async Task RefreshAccessToken()
    {
        var refreshRequest = new RefreshRequest() { RefreshToken = RetrieveToken() };

        var refreshResponse = await photoBoxClient.PostRefreshAsync(refreshRequest);

        configMonitor.CurrentValue.RefreshToken.Value = refreshResponse.RefreshToken;
    }
}
