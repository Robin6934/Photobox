using System.Net;
using System.Windows;
using Photobox.Lib.PhotoboxSettingsManager;
using Photobox.Lib.RestApi;

namespace Photobox.UI.Windows;

public partial class RegisterPhotoboxWindow : Window
{
    private readonly IPhotoboxSettingsManager _photoboxSettingsManager;
    private readonly TaskCompletionSource<bool> _completionSource = new();

    public Task<bool> Completion => _completionSource.Task;

    public RegisterPhotoboxWindow(IPhotoboxSettingsManager photoboxSettingsManager)
    {
        _photoboxSettingsManager = photoboxSettingsManager;
        InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string photoboxName = NameTextBox.Text.Trim();

        if (string.IsNullOrEmpty(photoboxName))
        {
            MessageBox.Show(
                "Please enter a Name.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            return;
        }

        try
        {
            await _photoboxSettingsManager.Register(new PhotoboxSettings(photoboxName));
            _completionSource.TrySetResult(true);
        }
        catch (ApiException)
        {
            MessageBox.Show(
                "An error occurred while registering the photo box. Please try again.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            _completionSource.TrySetResult(false);
        }
        finally
        {
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _completionSource.TrySetResult(false);
        Close();
    }
}
