using Photobox.Lib.AccessTokenManager;
using System.Windows;

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

        // Call login method if inputs are valid
        await _accessTokenManager.LoginAsync(email, password);

        if (!_accessTokenManager.LoggedIn)
        {
            MessageBox.Show("The login was not successful. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);    
        }
        
        Close();
    }


    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        OnLoginCanceled?.Invoke(this, e);
        this.Close();
    }
}