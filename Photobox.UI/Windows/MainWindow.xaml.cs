using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Camera;
using Photobox.Lib.ImageHandler;
using Photobox.UI.CountDown;
using Photobox.UI.ImageViewer;
using Photobox.WpfHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows;

namespace Photobox.UI.Windows;
public partial class MainWindow : Window, IHostedService
{
    private readonly ICamera camera;

    private readonly IImageViewer imageViewer;

    private readonly ILogger<MainWindow> logger;

    private readonly ICountDown countDown;

    private readonly IHostApplicationLifetime applicationLifetime;

    private readonly IImageHandler imageHandler;

    public MainWindow(ICamera camera, IImageViewer imageViewer, ILogger<MainWindow> logger, ICountDown countDown, IHostApplicationLifetime applicationLifetime, IImageHandler imageHandler)
    {
        InitializeComponent();

        this.camera = camera;

        this.imageViewer = imageViewer;

        this.logger = logger;

        this.countDown = countDown;

        this.applicationLifetime = applicationLifetime;

        this.imageHandler = imageHandler;

        countDown.Panel = GridLiveView;

        countDown.CountDownEarly += (s) =>
        {
            //camera.Focus();
        };

        countDown.CountDownExpired += async (s) =>
        {
            Image<Rgb24> image = await this.camera.TakePictureAsync();

            await this.imageViewer.ShowImage(image);
        };

        this.logger.LogInformation("Mainwindow created");

        this.camera.CameraStream += (o, i) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GridLiveView.Background = i.ToBitmapSource();
            });
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Start();
        Show();
        applicationLifetime.ApplicationStopping.Register(Close);
        logger.LogInformation("The main window has started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Close();
        logger.LogInformation("The main window has been closed.");
        return Task.CompletedTask;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        logger.LogInformation("The main window has been loaded.");
        SetCanvasSize();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetCanvasSize();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        camera.StopStream();
        applicationLifetime.StopApplication();
        logger.LogInformation("The main window is closing.");
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        logger.LogInformation("The main window has been closed.");
    }

    public void Start()
    {
        camera.ResilientConnect();
        camera.StartStream();
    }

    private void TakePictureButton_Click(object sender, RoutedEventArgs e)
    {
        countDown.StartCountDown();
    }

    /// <summary>
    /// Sets the Canvas to be in the middle of the screen
    /// </summary>
    private void SetCanvasSize()
    {
        // Actual image dimensions from the camera
        Rectangle rectangle = camera.PictureSize;
        double imageAspectRatio = (double)rectangle.Width / rectangle.Height;

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