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

    public override void Connect()
    {
        capture = new VideoCapture();
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1200);
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 800);
        capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);
    }

    public override void Disconnect()
    {
        capture?.Dispose();
        capture = null;
    }

    public override void Dispose()
    {
        capture?.Dispose();
        capture = null;
    }

    public override void StartStream()
    {
        logger.LogInformation("The stream of the WebCam has been started");
        LiveViewActive = true;
        Task.Run(() =>
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

    public override void StopStream()
    {
        LiveViewActive = false;
        logger.LogInformation("The stream of the WebCam has been stopped");
    }

    public override string TakePicture()
    {
        string imagePath = Folders.NewImagePath;

        Folders.CheckIfDirectoriesExistElseCreate();

        using Mat frame = new();
        capture?.Read(frame);
        frame.Save(imagePath);

        logger.LogInformation("New image taken with path {imagePath}", imagePath);

        return imagePath;
    }

    public override void Focus()
    {
        // blub
    }

    public static bool Connected()
    {
        using VideoCapture capture = new();
        return capture.IsOpened;
    }
}
