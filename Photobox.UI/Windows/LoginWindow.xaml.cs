using Photobox.UI.Lib.AccessTokenManager;
using System.Windows;

namespace Photobox.UI.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow(IAccessTokenManager accessTokenManager)
    {
        InitializeComponent();
    }
    
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text;
        string password = PasswordTextBox.Password;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}