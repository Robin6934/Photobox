using System.Net;
using System.Runtime.Versioning;
using AdysTech.CredentialManager;
using Photobox.Lib.RestApi;
using Exception = System.Exception;

namespace Photobox.Lib.AccessTokenManager;

[SupportedOSPlatform("Windows")]
public class AccessTokenManager(IClient photoBoxClient) : IAccessTokenManager
{
    private string? _accessToken;

    private DateTime _accessTokenExpiry = DateTime.Now;

    private const string RefreshTokenTarget = "PhotoboxRefreshToken";

    private string? RefreshToken
    {
        get
        {
            if (string.IsNullOrEmpty(field))
            {
                var credential = CredentialManager.GetCredentials(RefreshTokenTarget);
                field = credential?.Password!;
            }
            return field;
        }
        set
        {
            field = value;
            var credential = new NetworkCredential("Photobox", value);
            CredentialManager.SaveCredentials(RefreshTokenTarget, credential);
        }
    }

    public Task<string?> AccessToken => GetAccessToken();

    public bool LoggedIn =>
        !string.IsNullOrEmpty(_accessToken)
        && _accessTokenExpiry > DateTime.Now.Add(TimeSpan.FromSeconds(5));

    public bool RefreshTokenAvailable => !string.IsNullOrEmpty(RefreshToken);

    public async Task<bool> CheckIfRefreshTokenValid()
    {
        if (!RefreshTokenAvailable)
        {
            return false;
        }

        var refreshRequest = new RefreshRequest { RefreshToken = RefreshToken };

        AccessTokenResponse accessTokenResponse;

        try
        {
            accessTokenResponse = await photoBoxClient.PostRefreshAsync(refreshRequest);
        }
        catch (ApiException e) when (e.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            // The token is not valid, the user needs to log in
            Logout();
            return false;
        }
        catch (Exception e)
        {
            Logout();
            throw new InvalidOperationException(
                "An error occurred while trying to access the refresh token.",
                e
            );
        }

        _accessToken = accessTokenResponse.AccessToken;

        RefreshToken = accessTokenResponse.RefreshToken;

        _accessTokenExpiry = DateTime.Now.AddSeconds(accessTokenResponse.ExpiresIn);

        return true;
    }

    public async Task LoginAsync(string email, string password)
    {
        var loginRequest = new LoginRequest { Email = email, Password = password };

        AccessTokenResponse loginResponse;
        try
        {
            loginResponse = await photoBoxClient.PostLoginAsync(loginRequest, false, false);
        }
        catch (ApiException e) when (e.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new CredentialValidationException("The username or password are wrong.", e);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                "An error occurred while trying to access the refresh token.",
                e
            );
        }

        _accessToken = loginResponse.AccessToken;

        RefreshToken = loginResponse.RefreshToken;

        _accessTokenExpiry = DateTime.Now.AddSeconds(loginResponse.ExpiresIn);
    }

    public void Logout()
    {
        _accessToken = null;
        _accessTokenExpiry = DateTime.Now;

        var credential = CredentialManager.GetICredential(RefreshTokenTarget);
        if (credential is not null)
        {
            CredentialManager.RemoveCredentials(RefreshTokenTarget);
        }
    }

    private async Task<string?> GetAccessToken()
    {
        if (LoggedIn)
        {
            return _accessToken;
        }

        if (string.IsNullOrEmpty(RefreshToken))
        {
            return null;
        }

        var refreshTokenRequest = new RefreshRequest { RefreshToken = RefreshToken };

        var refreshTokenResponse = await photoBoxClient
            .PostRefreshAsync(refreshTokenRequest)
            .ConfigureAwait(false);

        _accessToken = refreshTokenResponse.AccessToken;
        RefreshToken = refreshTokenResponse.RefreshToken;

        _accessTokenExpiry = DateTime.Now.AddSeconds(refreshTokenResponse.ExpiresIn);

        return _accessToken;
    }
}
