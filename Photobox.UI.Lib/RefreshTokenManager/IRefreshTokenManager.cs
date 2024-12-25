namespace Photobox.UI.Lib.CredentialManager;
public interface IRefreshTokenManager
{
    public bool LoggedIn { get; }

    public Task LoginAsync(string email, string password);

    public string RetrieveToken();
}
