using System.Windows;
using Photobox.Lib.AccessTokenManager;

namespace Photobox.UI.Windows;

public partial class LoginWindow : Window
{
    public delegate void LoginCanceled(object sender, RoutedEventArgs e);

    public event LoginCanceled? OnLoginCanceled;

    private readonly IAccessTokenManager _accessTokenManager;

    public LoginWindow(IAccessTokenManager accessTokenManager)
    {
        _accessTokenManager = accessTokenManager;

        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text.Trim();
        string password = PasswordTextBox.Password.Trim();

        // Validate inputs
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            // Construct a single error message
            string errorMessage = string.Empty;

            if (string.IsNullOrEmpty(email))
                errorMessage += "Please enter an email address.\n";
            if (string.IsNullOrEmpty(password))
                errorMessage += "Please enter a password.";

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            await _accessTokenManager.LoginAsync(email, password);
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
                Application.Current.Shutdown();
            }
            return;
        }
        catch (Exception)
        {
            MessageBox.Show(
                "An error occurred while logging in. Please check your internet connection and try again.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            Application.Current.Shutdown();
            return;
        }

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        OnLoginCanceled?.Invoke(this, e);
        this.Close();
    }
}
