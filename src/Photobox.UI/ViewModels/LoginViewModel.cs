using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Photobox.UI.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    public string Email
    {
        get;
        set { SetField(ref field, value); }
    }

    public string Password
    {
        get;
        set { SetField(ref field, value); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task LoginAsync()
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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
