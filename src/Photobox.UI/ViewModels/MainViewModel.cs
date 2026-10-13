using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Photobox.UI.Commands;
using Photobox.UI.ImageViewer;
using Photobox.UI.Lib.Camera;
using Photobox.UI.Lib.ImageManager;
using SixLabors.ImageSharp;

namespace Photobox.UI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ICamera _camera;

    private readonly IImageViewer _imageViewer;

    private readonly IImageManager _imageManager;

    public ICommand TakePictureCommand { get; }

    public WriteableBitmap VideoFrame { get; }

    public int Countdown
    {
        get;
        private set { SetField(ref field, value); }
    }

    public bool IsCountdownVisible => Countdown > 0;

    public MainViewModel(ICamera camera, IImageViewer imageViewer, IImageManager imageManager)
    {
        _camera = camera;
        _imageViewer = imageViewer;
        _imageManager = imageManager;

        _camera.ResilientConnect();

        VideoFrame = new WriteableBitmap(
            (int)camera.ImageAspectRatio.WidthPixels,
            (int)camera.ImageAspectRatio.HeightPixels,
            96,
            96,
            PixelFormats.Bgr32,
            null
        );

        TakePictureCommand = new AsyncRelayCommand(TakePictureAsync);

        _camera.CameraStream += CameraOnCameraStream;

        _camera.StartStream();
    }

    private void CameraOnCameraStream(object sender, Stream img)
    {
        img.Seek(0, SeekOrigin.Begin);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = img;
        bitmap.EndInit();
        bitmap.Freeze(); // Freeze for cross-thread use

        Application.Current.Dispatcher.Invoke(() =>
        {
            VideoFrame.Lock();
            bitmap.CopyPixels(
                new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                VideoFrame.BackBuffer,
                VideoFrame.BackBufferStride * VideoFrame.PixelHeight,
                VideoFrame.BackBufferStride
            );
            VideoFrame.AddDirtyRect(
                new Int32Rect(0, 0, VideoFrame.PixelWidth, VideoFrame.PixelHeight)
            );
            VideoFrame.Unlock();
        });
    }

    private async Task TakePictureAsync()
    {
        await RunCountdown(5);

        var image = await _camera.TakePictureAsync();

        ImageViewResult result = await _imageViewer.ShowImage(image);

        MemoryStream stream = new();

        await image.SaveAsJpegAsync(stream);

        stream.Position = 0;

        switch (result)
        {
            case ImageViewResult.Save:
                await _imageManager.SaveAsync(image);
                break;
            case ImageViewResult.Print:
                await _imageManager.PrintAndSaveAsync(image);
                break;
            case ImageViewResult.Delete:
                await _imageManager.DeleteAsync(image);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }

    private async Task RunCountdown(int seconds)
    {
        for (Countdown = seconds; Countdown > 0; Countdown--)
            await Task.Delay(1000);

        Countdown = 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
