using System.Net;
using AdysTech.CredentialManager;
using NSubstitute;
using Photobox.Lib.RestApi;
using Photobox.Lib.AccessTokenManager;
using Shouldly;

namespace Photobox.Tests.AccessTokenManager;

public class AccessTokenManagerTests
{
    private readonly IClient _photoBoxClient;
    private readonly IPhotoBoxClient _photoBoxApiClient;
    private IAccessTokenManager _accessTokenManager;

    public AccessTokenManagerTests()
    {
        _photoBoxClient = Substitute.For<IClient>();
        _photoBoxApiClient = Substitute.For<IPhotoBoxClient>();
        _accessTokenManager = new Lib.AccessTokenManager.AccessTokenManager(_photoBoxClient, _photoBoxApiClient);
    }

    [Fact]
    public async Task LoginAsync_ShouldBeLoggedIn()
    {
        // Arrange

        string email = "test@example.com";
        string password = "password123";
        var loginResponse = new AccessTokenResponse
        {
            AccessToken = "access-token", RefreshToken = "refresh-token", ExpiresIn = 3600
        };

        _photoBoxClient.PostLoginAsync(Arg.Any<LoginRequest>(), false, false)
            .Returns(Task.FromResult(loginResponse));

        // Act
        await _accessTokenManager.LoginAsync(email, password);

        // Assert
        _accessTokenManager.LoggedIn.ShouldBeTrue();
        (await _accessTokenManager.AccessToken).ShouldBe("access-token");
        _accessTokenManager.RefreshTokenAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAccessToken_ShouldRefreshToken_WhenAccessTokenIsExpired()
    {
        // Arrange
        var refreshTokenResponse = new AccessTokenResponse
        {
            AccessToken = "new-access-token", RefreshToken = "new-refresh-token", ExpiresIn = 3600
        };

        _photoBoxClient.PostRefreshAsync(Arg.Any<RefreshRequest>())
            .Returns(Task.FromResult(refreshTokenResponse));

        string expiredToken = "expired-access-token";
        string refreshToken = "valid-refresh-token";

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        // Assert
        accessToken.ShouldBe("new-access-token");
    }

    [Fact]
    public void RefreshTokenAvailable_ShouldReturnTrue_IfRefreshTokenIsPresent()
    {
        // Arrange

        CredentialManager.SaveCredentials("PhotoboxRefreshToken",
            new NetworkCredential("Photobox", "valid-refresh-token"));

        // Act
        bool result = _accessTokenManager.RefreshTokenAvailable;

        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void RefreshTokenAvailable_ShouldReturnFalse_IfNoRefreshTokenIsPresent()
    {
        // Arrange

        var credential = CredentialManager.GetICredential("PhotoboxRefreshToken");
        if (credential is not null)
        {
            CredentialManager.RemoveCredentials("PhotoboxRefreshToken");
        }

        // Act
        bool result = _accessTokenManager.RefreshTokenAvailable;

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task LoggedIn_ShouldReturnFalse_WhenAccessTokenIsExpired()
    {
        // Arrange

        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access-token", RefreshToken = "refresh-token", ExpiresIn = 6
        };

        _photoBoxClient.PostLoginAsync(Arg.Any<LoginRequest>(), false, false)
            .Returns(Task.FromResult(accessTokenResponse));
        
        await _accessTokenManager.LoginAsync("email", "password");
        
        _accessTokenManager.LoggedIn.ShouldBeTrue();

        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        // Act
        bool loggedIn = _accessTokenManager.LoggedIn;

        // Assert
        loggedIn.ShouldBeFalse();
    }
    
    [Fact]
    public async Task AccessToken_ShouldBeUpdated_WhenAccessTokenIsExpired()
    {
        // Arrange

        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access-token", RefreshToken = "refresh-token", ExpiresIn = 6
        };

        _photoBoxClient.PostLoginAsync(Arg.Any<LoginRequest>(), false, false)
            .Returns(Task.FromResult(accessTokenResponse));
        
        await _accessTokenManager.LoginAsync("email", "password");
        
        _accessTokenManager.LoggedIn.ShouldBeTrue();
        
        var refreshTokenResponse = new AccessTokenResponse
        {
            AccessToken = "new-access-token", RefreshToken = "new-refresh-token", ExpiresIn = 6
        };
        
        _photoBoxClient.PostRefreshAsync(Arg.Any<RefreshRequest>())
            .Returns(Task.FromResult(refreshTokenResponse));

        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        // Assert
        accessToken.ShouldBe("new-access-token");
    }
}