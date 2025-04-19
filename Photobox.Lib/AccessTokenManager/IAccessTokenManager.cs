namespace Photobox.Lib.AccessTokenManager;

public interface IAccessTokenManager
{
    /// <summary>
    /// Gets the current access token, the token is automatically refreshed if it is expired.
    /// If no RefreshToken is available, null is returned.
    /// </summary>
    Task<string?> AccessToken { get; }
    
    /// <summary>
    /// Returns, if the current token is valid.
    /// </summary>
    bool LoggedIn { get; }
    
    /// <summary>
    /// Returns, if a RefreshToken is available.
    /// If no refresh token is available, the user has to login again.
    /// </summary>
    bool RefreshTokenAvailable { get; }

    /// <summary>
    /// Checks if the Refreshtoken is valid and retrieves a new access token with it.
    /// </summary>
    /// <exception cref="CredentialValidationException">Refresh token not valid or not available.</exception>
    /// <exception cref="InvalidOperationException">Another error happened.</exception>
    Task CheckIfRefreshTokenValid();
    
    /// <summary>
    /// Logs in the user with the given email and password.
    /// </summary>
    /// <param name="email">The users email address.</param>
    /// <param name="password">The password for the given email address.</param>
    /// <exception cref="CredentialValidationException">Username or password are wrong.</exception>
    /// <exception cref="InvalidOperationException">Another error happened.</exception>
    Task LoginAsync(string email, string password);
    
    /// <summary>
    /// Logs the user out, invalidating the current access and refresh tokens.
    /// </summary>
    void Logout();
}