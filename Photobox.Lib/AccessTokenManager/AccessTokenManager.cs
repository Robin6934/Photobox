using AdysTech.CredentialManager;
using Photobox.Lib.RestApi;
using System.Net;

namespace Photobox.Lib.AccessTokenManager;

public class AccessTokenManager(IClient photoBoxClient, IPhotoBoxClient client) : IAccessTokenManager
{
    private string? _accessToken;
    
    private DateTime _accessTokenExpiry = DateTime.Now;
    
    private const string RefreshTokenTarget = "PhotoboxRefreshToken";

    private string? RefreshToken
    {
        get
        {
            var credential = CredentialManager.GetCredentials(RefreshTokenTarget);
            return credential?.Password;
        }
        set
        {
            var credential = new NetworkCredential("Photobox", value);
            CredentialManager.SaveCredentials(RefreshTokenTarget, credential);
        }
    }

    public Task<string?> AccessToken => GetAccessToken();
    
    public bool LoggedIn => !string.IsNullOrEmpty(_accessToken) && _accessTokenExpiry > DateTime.Now.Add(TimeSpan.FromSeconds(5));
    
    public bool RefreshTokenAvailable => !string.IsNullOrEmpty(RefreshToken);

    public async Task LoginAsync(string email, string password)
    {
        var loginRequest = new LoginRequest()
        {
            Email = email, 
            Password = password
        };

        var loginResponse = await photoBoxClient.PostLoginAsync(loginRequest, false, false);

        _accessToken = loginResponse.AccessToken;
        
        RefreshToken = loginResponse.RefreshToken;
        
        _accessTokenExpiry = DateTime.Now.AddSeconds(loginResponse.ExpiresIn);
    }
    
    public void Logout()
    {
        _accessToken = null;
        _accessTokenExpiry = DateTime.Now;
    }
    
    private async Task<string?> GetAccessToken()
    {
        if(LoggedIn)
        {
            return _accessToken;
        }

        if (string.IsNullOrEmpty(RefreshToken))
        {
            return null;
        }
        
        var refreshTokenRequest = new RefreshRequest()
        {
            RefreshToken = RefreshToken
        };
        
        var refreshTokenResponse = await photoBoxClient.PostRefreshAsync(refreshTokenRequest);
        
        _accessToken = refreshTokenResponse.AccessToken;
        RefreshToken = refreshTokenResponse.RefreshToken;
        
        _accessTokenExpiry = DateTime.Now.AddSeconds(refreshTokenResponse.ExpiresIn);

        return _accessToken;
    }
}
