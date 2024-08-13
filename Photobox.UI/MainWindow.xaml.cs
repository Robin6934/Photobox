using Photobox.Lib.Camera;
using Photobox.WpfHelpers;
using System.Drawing;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Photobox.UI;
public partial class MainWindow : Window
{
    private readonly ICamera camera;

    public MainWindow(ICamera cam)
    {
        InitializeComponent();

        camera = cam;

        camera.CameraStream += Camera_CameraStream;
        Closing += MainWindow_Closing;
        SizeChanged += MainWindow_SizeChanged;
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

    private void Camera_CameraStream(object sender, Bitmap img)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CanvasLiveView.Background = new ImageBrush(img.ToBitmapSource());
        });
    }

    private async void TakePictureButton_Click(object sender, RoutedEventArgs e)
    {
        await camera.TakePictureAsync();
    }

    /// <summary>
    /// Sets the Canvas to be in the middle of the screen
    /// </summary>
    private void SetCanvasSize()
    {
        // Calculate the desired canvas size based on the window's height
        double windowHeight = this.ActualHeight;
        double canvasHeight = windowHeight;
        double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

        // Set the canvas size
        CanvasLiveView.Width = canvasWidth;
        CanvasLiveView.Height = canvasHeight;
    }
}