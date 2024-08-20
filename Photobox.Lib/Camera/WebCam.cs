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
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1080);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);
            capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);
        });
    }

    public override async Task DisconnectAsync()
    {
        capture.Dispose();
        capture = null!;
        await Task.CompletedTask;
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

        Folders.CheckIfDirectoriesExistElseCreate();

        await Task.Run(() =>
        {
            using Mat frame = new();
            capture.Read(frame);
            frame.Save(imagePath);
        });

        return imagePath;
    }

    public override async Task FocusAsync()
    {
        await Task.CompletedTask;
    }
}
