using System.Windows;
using Microsoft.Extensions.Hosting;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.RestApi;
using Exception = System.Exception;

namespace Photobox.UI.Windows;

public enum LoginResult
{
    Success,
    Canceled,
    Error,
}

public partial class LoginWindow : Window
{
    private readonly IAccessTokenManager _accessTokenManager;
    private readonly TaskCompletionSource<LoginResult> _completionSource = new();

    public Task<LoginResult> Completion => _completionSource.Task;

    public LoginWindow(
        IAccessTokenManager accessTokenManager,
        IHostApplicationLifetime applicationLifetime
    )
    {
        _accessTokenManager = accessTokenManager;
        InitializeComponent();

        applicationLifetime.ApplicationStopping.Register(() =>
        {
            if (!_completionSource.Task.IsCompleted)
            {
                _completionSource.TrySetResult(LoginResult.Canceled);
                Close();
            }
        });
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text.Trim();
        string password = PasswordTextBox.Password.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            var errorMessage = string.Join(
                "\n",
                string.IsNullOrEmpty(email) ? "Please enter an email address." : null,
                string.IsNullOrEmpty(password) ? "Please enter a password." : null
            );

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            await _accessTokenManager.LoginAsync(email, password);
            _completionSource.TrySetResult(LoginResult.Success);
        }
        catch (CredentialValidationException)
        {
            var result = MessageBox.Show(
                "Either the username or password is incorrect. Would you like to try again?",
                "Login Failed",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.No)
            {
                CancelLogin();
            }
        }
        catch (Exception)
        {
            MessageBox.Show(
                "An error occurred while logging in. Please check your internet connection and try again.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            CancelLogin();
        }
        finally
        {
            if (_completionSource.Task.IsCompleted)
            {
                Close();
            }
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CancelLogin();
    }

    private void CancelLogin()
    {
        _accessTokenManager.Logout();
        _completionSource.TrySetResult(LoginResult.Canceled);
        Close();
    }
}
