using Photobox.Lib.Camera;
using Photobox.WpfHelpers;
using System.Drawing;
using System.Windows;
using System.Windows.Media;


namespace Photobox.UI;
public partial class MainWindow : Window
{
    private readonly ICamera camera;
    public MainWindow(ICamera cam)
    {
        camera = cam;
        camera.CameraStream += Camera_CameraStream;
        camera.ConnectAsync().Wait();
        _ = camera.StartStreamAsync();
        InitializeComponent();
    }

    private void Camera_CameraStream(object sender, Bitmap img)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CanvasLiveView.Background = new ImageBrush(img.ToBitmapSource());
        });
    }
}