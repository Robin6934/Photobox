using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Photobox.Lib.Camera;

public class WebCam(ILogger<WebCam> logger, IHostApplicationLifetime applicationLifetime) : CameraBase
{
    private VideoCapture? capture = default;

    private readonly ILogger<WebCam> logger = logger;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

    public override Task ConnectAsync()
    {
        capture = new VideoCapture();
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1200);
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 800);
        capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);

        return Task.CompletedTask;
    }

    public override Task DisconnectAsync()
    {
        capture?.Dispose();
        capture = null;
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        capture?.Dispose();
        capture = null;
    }

    public override Task StartStreamAsync()
    {
        logger.LogInformation("The stream of the WebCam has been started");
        LiveViewActive = true;
        return Task.Run(() =>
        {
            using Mat frame = new();
            while (!applicationLifetime.ApplicationStopping.IsCancellationRequested
                && LiveViewActive)
            {
                capture?.Read(frame);

                Span<byte> data = frame.ToImage<Rgb, byte>(false).ToJpegData();

                OnNewStreamImage(new MemoryStream(data.ToArray()));
            }
        }, applicationLifetime.ApplicationStopping);
    }

    public override Task StopStreamAsync()
    {
        LiveViewActive = false;
        logger.LogInformation("The stream of the WebCam has been stopped");
        return Task.CompletedTask;
    }

    public override async Task<string> TakePictureAsync()
    {
        string imagePath = Folders.NewImagePath;

        Folders.CheckIfDirectoriesExistElseCreate();

        using Mat frame = new();
        capture?.Read(frame);
        frame.Save(imagePath);

        logger.LogInformation("New image taken with path {imagePath}", imagePath);

        await Task.CompletedTask;

        return imagePath;
    }

    public override Task FocusAsync()
    {
        return Task.CompletedTask;
    }

    public static bool Connected()
    {
        VideoCapture capture = new();
        return capture.IsOpened;
    }
}
