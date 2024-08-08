using Photobox.Lib.Camera;
using Photobox.WpfHelpers;
using System.Drawing;
using System.Net.Http;
using System.Windows;
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
}