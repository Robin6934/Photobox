using System.Net;
using AdysTech.CredentialManager;
using NSubstitute;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.RestApi;
using Shouldly;

namespace Photobox.Lib.Test;

public class AccessTokenManagerTests
{
    private readonly IClient _photoBoxClient;
    private IAccessTokenManager _accessTokenManager;

    public AccessTokenManagerTests()
    {
        _photoBoxClient = Substitute.For<IClient>();
        _accessTokenManager = new Lib.AccessTokenManager.AccessTokenManager(_photoBoxClient);
    }

    [Fact]
    public async Task LoginAsync_ShouldBeLoggedIn()
    {
        // Arrange

        string email = "test@example.com";
        string password = "password123";
        var loginResponse = new AccessTokenResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
        };

        _photoBoxClient
            .PostApiLoginAsync(
                Arg.Any<LoginRequest>(),
                false,
                false,
                TestContext.Current.CancellationToken
            )
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
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresIn = 3600,
        };

        _photoBoxClient
            .PostApiRefreshAsync(Arg.Any<RefreshRequest>(), TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(refreshTokenResponse));

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        // Assert
        accessToken.ShouldBe("new-access-token");
    }

    [Fact]
    public void RefreshTokenAvailable_ShouldReturnTrue_IfRefreshTokenIsPresent()
    {
        // Arrange

        CredentialManager.SaveCredentials(
            "PhotoboxRefreshToken",
            new NetworkCredential("Photobox", "valid-refresh-token")
        );

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
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 6,
        };

        _photoBoxClient
            .PostApiLoginAsync(
                Arg.Any<LoginRequest>(),
                false,
                false,
                TestContext.Current.CancellationToken
            )
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
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 6,
        };

        _photoBoxClient
            .PostApiLoginAsync(
                Arg.Any<LoginRequest>(),
                false,
                false,
                TestContext.Current.CancellationToken
            )
            .Returns(Task.FromResult(accessTokenResponse));

        await _accessTokenManager.LoginAsync("email", "password");

        _accessTokenManager.LoggedIn.ShouldBeTrue();

        var refreshTokenResponse = new AccessTokenResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresIn = 6,
        };

        _photoBoxClient
            .PostApiRefreshAsync(Arg.Any<RefreshRequest>(), TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(refreshTokenResponse));

        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        // Assert
        accessToken.ShouldBe("new-access-token");
    }

    [Fact]
    public async Task AccessToken_ShouldBeNull_WhenNotLoggedIn()
    {
        // Arrange
        var credential = CredentialManager.GetICredential("PhotoboxRefreshToken");
        if (credential is not null)
        {
            CredentialManager.RemoveCredentials("PhotoboxRefreshToken");
        }

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        // Assert
        accessToken.ShouldBeNull();
    }

    [Fact]
    public async Task AccessTokenManager_Logout_ShouldDeleteBothTokens()
    {
        // Arrange
        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 6,
        };

        _photoBoxClient
            .PostApiLoginAsync(
                Arg.Any<LoginRequest>(),
                false,
                false,
                TestContext.Current.CancellationToken
            )
            .Returns(Task.FromResult(accessTokenResponse));

        await _accessTokenManager.LoginAsync("", "");

        (await _accessTokenManager.AccessToken).ShouldBe("access-token");

        _accessTokenManager.Logout();

        // Act
        string? accessToken = await _accessTokenManager.AccessToken;

        var credential = CredentialManager.GetICredential("PhotoboxRefreshToken");

        // Assert
        accessToken.ShouldBeNull();
        credential.ShouldBeNull();
    }
}
