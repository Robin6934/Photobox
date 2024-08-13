using Emgu.CV;

namespace Photobox.Lib.Camera;

public class WebCam : CameraBase
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private VideoCapture capture = null!;

    public override Task ConnectAsync()
    {
        return Task.Run(() =>
        {
            capture = new VideoCapture();
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
            capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);
        });
    }

    public override Task DisconnectAsync()
    {
        return Task.Run(() =>
        {
            capture.Dispose();
            capture = null!;
        });
    }

    public override void Dispose()
    {
        cancellationTokenSource.Dispose();
    }

    public override Task StartStreamAsync()
    {
        return Task.Run(() =>
        {
            using Mat frame = new();
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                capture.Read(frame);
                OnNewStreamImage(frame.ToBitmap());
            }
        }, cancellationTokenSource.Token);
    }

    public override Task StopStreamAsync()
    {
        return Task.Run(cancellationTokenSource.Cancel);
    }

    public override async Task<string> TakePictureAsync()
    {
        string imagePath = Folders.NewImagePath;

        await Task.Run(() =>
        {
            using Mat frame = new();
            capture.Read(frame);
            frame.Save(imagePath);
            OnPictureTaken(frame.ToBitmap());
        });

        return imagePath;
    }

    public override Task FocusAsync()
    {
        return Task.Run(() =>
        {
            throw new NotImplementedException();
        });
    }
}
