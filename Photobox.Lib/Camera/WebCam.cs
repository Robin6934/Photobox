using Emgu.CV;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Emgu.CV.Structure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;

namespace Photobox.Lib.Camera;

public class WebCam(ILogger<WebCam> logger, IHostApplicationLifetime applicationLifetime) : CameraBase
{
    private VideoCapture capture = null!;

    private readonly ILogger<WebCam> logger = logger;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

    private bool liveViewActive = false;

    public override Task ConnectAsync()
    {
        capture = new VideoCapture();
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1080);
        capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);
        capture.Set(Emgu.CV.CvEnum.CapProp.Fps, 60);

        return Task.CompletedTask;
    }

    public override Task DisconnectAsync()
    {
        capture.Dispose();
        capture = null!;
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        capture.Dispose();
        capture = null!;
    }

    public override Task StartStreamAsync()
    {
        logger.LogInformation("The stream of the WebCam has been started}");
        liveViewActive = true;
        return Task.Run(() =>
        {
            using Mat frame = new();
            while (!applicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                capture.Read(frame);
                Stopwatch sw = Stopwatch.StartNew();
                Span<byte> data = frame.ToImage<Rgb, byte>(false).ToJpegData();
                sw.Stop();
                Debug.WriteLine(sw.Elapsed.TotalMilliseconds);
                ImageInfo info = Image.Identify(data);

                OnNewStreamImage(new MemoryStream(data.ToArray()));
            }
        }, applicationLifetime.ApplicationStopping);
    }

    public override async Task StopStreamAsync()
    {
        liveViewActive = false;
        logger.LogInformation("The stream of the WebCam has been stopped}");
        return Task.CompletedTask;
    }

    public override async Task<string> TakePictureAsync()
    {
        string imagePath = Folders.NewImagePath;

        Folders.CheckIfDirectoriesExistElseCreate();

        using Mat frame = new();
        capture.Read(frame);
        frame.Save(imagePath);

        return imagePath;
    }

    public override Task FocusAsync()
    {
        return Task.CompletedTask;
    }

    public static bool CameraConnected()
    {
        VideoCapture capture = new();
        return capture.IsOpened;
    }
}
