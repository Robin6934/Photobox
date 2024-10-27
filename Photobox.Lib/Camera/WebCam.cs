using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.Camera;

public class WebCam(ILogger<WebCam> logger, IHostApplicationLifetime applicationLifetime) : CameraBase(logger)
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
        logger.LogInformation("[WebCam] has been connected.");
    }

    public override void Disconnect()
    {
        capture?.Dispose();
        capture = null;
        logger.LogInformation("[WebCam] has been disconnected.");
    }

    public override void Dispose()
    {
        capture?.Dispose();
        capture = null;
        logger.LogInformation("[WebCam] has been disposed.");
    }

    public override void StartStream()
    {
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
        logger.LogInformation("[WebCam] liveView started.");
    }

    public override void StopStream()
    {
        LiveViewActive = false;
        logger.LogInformation("[WebCam] liveView stopped.");
    }

    public override async Task<Image<Rgb24>> TakePictureAsync()
    {
        using Mat frame = new();
        capture?.Read(frame);

        logger.LogInformation("[WebCam] picture has been taken.");

        await Task.CompletedTask;

        return Image.Load<Rgb24>(frame.ToImage<Rgb, byte>(false).ToJpegData());
    }

    public override void Focus()
    {
        logger.LogInformation("[WebCam] has focused.");
    }

    public static bool Connected()
    {
        using VideoCapture capture = new();
        return capture.IsOpened;
    }
}
