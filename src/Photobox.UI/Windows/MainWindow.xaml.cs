using System.IO;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.PhotoboxSettingsManager;
using Photobox.Lib.RestApi;
using Photobox.UI.CountDown;
using Photobox.UI.ImageViewer;
using Photobox.UI.Lib.Camera;
using Photobox.UI.Lib.ImageHandler;
using Photobox.UI.Lib.ImageManager;
using Photobox.WpfHelpers;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Exception = System.Exception;

namespace Photobox.UI.Windows;

public partial class MainWindow : Window, IHostedService
{
    private readonly ICamera _camera;

    private readonly ILogger<MainWindow> _logger;

    private readonly ICountDown _countDown;

    private readonly IHostApplicationLifetime _applicationLifetime;

    private readonly IAccessTokenManager _accessTokenManager;

    private readonly IPhotoboxSettingsManager _photoboxSettingsManager;

    public MainWindow(
        ICamera camera,
        IImageViewer imageViewer,
        ILogger<MainWindow> logger,
        ICountDown countDown,
        IHostApplicationLifetime applicationLifetime,
        IImageHandler imageHandler,
        IImageManager imageManager,
        IAccessTokenManager accessTokenManager,
        IPhotoBoxClient photoBoxClient,
        IPhotoboxSettingsManager photoboxSettingsManager
    )
    {
        InitializeComponent();

        this._camera = camera;

        IImageViewer imageViewer1 = imageViewer;

        _logger = logger;

        _countDown = countDown;

        _applicationLifetime = applicationLifetime;

        _accessTokenManager = accessTokenManager;

        _photoboxSettingsManager = photoboxSettingsManager;

        countDown.Panel = GridLiveView;

        countDown.CountDownEarly += (s) =>
        {
            camera.Focus();
        };

        countDown.CountDownExpired += async (o) =>
        {
            Image<Rgb24> image = await this._camera.TakePictureAsync();

            ImageViewResult result = await imageViewer1.ShowImage(image);

            BorderText.Visibility = Visibility.Visible;

            MemoryStream stream = new();

            await image.SaveAsJpegAsync(stream);

            stream.Position = 0;

            switch (result)
            {
                case ImageViewResult.Save:
                    await imageManager.SaveAsync(image);
                    break;
                case ImageViewResult.Print:
                    await imageManager.PrintAndSaveAsync(image);
                    break;
                case ImageViewResult.Delete:
                    await imageManager.DeleteAsync(image);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        };

        this._logger.LogInformation("Mainwindow created");

        this._camera.CameraStream += CameraStreamHandler;
    }

    private void CameraStreamHandler(object o, Stream s)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            GridLiveView.Background = s.ToBitmapSource();
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Start();
        Show();
        _applicationLifetime.ApplicationStopping.Register(Close);
        _logger.LogInformation("The main window has started.");

        try
        {
            bool refreshTokenValid = await _accessTokenManager.CheckIfRefreshTokenValid();

            if (!refreshTokenValid)
            {
                await LoginAsync();
            }

            if (!await _photoboxSettingsManager.CheckIfPhotoboxIsRegistered())
            {
                await RegisterAsync();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(
                "An error occurred while logging in. Please check your internet connection and try again.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private async Task<bool> LoginAsync()
    {
        var loginWindow = new LoginWindow(_accessTokenManager, _applicationLifetime);
        loginWindow.Show();

        var result = await loginWindow.Completion;

        if (result == LoginResult.Success)
        {
            return true;
        }

        _applicationLifetime.StopApplication();
        return false;
    }

    private async Task RegisterAsync()
    {
        var registerWindow = new RegisterPhotoboxWindow(_photoboxSettingsManager);
        registerWindow.Show();

        bool success = await registerWindow.Completion;

        if (!success)
        {
            _applicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Close();
        _logger.LogInformation("The main window has been closed.");
        return Task.CompletedTask;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("The main window has been loaded.");
        SetCanvasSize();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetCanvasSize();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        this._camera.CameraStream -= CameraStreamHandler;
        _camera.Dispose();
        _applicationLifetime.StopApplication();
        _logger.LogInformation("The main window is closing.");
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _logger.LogInformation("The main window has been closed.");
    }

    public void Start()
    {
        _camera.ResilientConnect();
        _camera.StartStream();
    }

    private void TakePictureButton_Click(object sender, RoutedEventArgs e)
    {
        BorderText.Visibility = Visibility.Hidden;
        _countDown.StartCountDown();
    }

    /// <summary>
    /// Sets the Canvas to be in the middle of the screen
    /// </summary>
    private void SetCanvasSize()
    {
        double imageAspectRatio = _camera.ImageAspectRatio.Ratio;

        // Available window dimensions
        double availableHeight = ActualHeight;
        double availableWidth = ActualWidth;

        // Set initial canvas size based on window height and image aspect ratio
        double canvasHeight = availableHeight;
        double canvasWidth = canvasHeight * imageAspectRatio;

        // Adjust if the canvas width exceeds available width
        if (canvasWidth > availableWidth)
        {
            // Recalculate canvas height based on available width while maintaining the aspect ratio
            canvasWidth = availableWidth;
            canvasHeight = canvasWidth / imageAspectRatio;
        }

        // Set the canvas size to fit the image
        GridLiveView.Width = canvasWidth;
        GridLiveView.Height = canvasHeight;

        // Optional: Adjust the grid column and row to take a third of the available space if required
        GridColumn.Width = new GridLength(availableWidth / 3.0);
        GridRow.Height = new GridLength(availableHeight / 3.0);
    }
}
