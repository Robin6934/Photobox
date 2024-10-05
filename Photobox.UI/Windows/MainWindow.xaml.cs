using Microsoft.Extensions.Logging;
using Photobox.Lib.Camera;
using Photobox.UI.ImageViewer;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobox.UI.Windows;
public partial class MainWindow : Window//, IHostedService
{
    private readonly ICamera camera;

    private readonly IImageViewer imageViewer;

    private readonly ILogger<MainWindow> logger;

    public MainWindow(ICamera cam, IImageViewer viewer, ILogger<MainWindow> log)
    {
        InitializeComponent();

        camera = cam;

        imageViewer = viewer;

        logger = log;

        logger.LogInformation("Mainwindow created");

        camera.CameraStream += (o, i) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GridLiveView.Background = new ImageBrush(ConvertStreamToBitmapSource(i));
            });
        };
    }

    public BitmapSource ConvertStreamToBitmapSource(Stream stream)
    {
        // Ensure the stream is at the beginning before reading it.
        stream.Seek(0, SeekOrigin.Begin);

        BitmapImage bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the data immediately, so stream doesn't stay open.
        bitmap.StreamSource = stream;
        bitmap.EndInit();

        bitmap.Freeze(); // Freeze the bitmap to make it thread-safe and immutable

        return bitmap;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        SetCanvasSize();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetCanvasSize();
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        await camera.StopStreamAsync();
        await new HttpClient().GetAsync("https://localhost:7176/api/Application/ShutDown");
    }

    public async Task Start()
    {
        await camera.ConnectAsync();
        _ = camera.StartStreamAsync();
    }

    private async void TakePictureButton_Click(object sender, RoutedEventArgs e)
    {
        string imagePath = await camera.TakePictureAsync();

        await imageViewer.ShowImage(imagePath);
    }

    /// <summary>
    /// Sets the Canvas to be in the middle of the screen
    /// </summary>
    private void SetCanvasSize()
    {
        // Calculate the desired canvas size based on the window's height
        double canvasHeight = ActualHeight;
        double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

        // If the calculated canvas width exceeds the window's width, adjust the size
        if (canvasWidth > ActualWidth)
        {
            // Recalculate canvas height based on the actual window width while maintaining the 3:2 aspect ratio
            canvasWidth = ActualWidth;
            canvasHeight = (canvasWidth / 3) * 2;
        }

        // Set the canvas size
        GridLiveView.Width = canvasWidth;
        GridLiveView.Height = canvasHeight;

        GridColumn.Width = new GridLength(ActualWidth / 3.0);
        GridRow.Height = new GridLength(ActualHeight / 3.0);
    }
}