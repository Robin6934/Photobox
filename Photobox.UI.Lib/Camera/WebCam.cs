using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Photobox.Lib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Camera;

public class WebCam(ILogger<WebCam> logger, IHostApplicationLifetime applicationLifetime) : CameraBase(logger)
{
    private VideoCapture? _capture = default;

    private readonly ILogger<WebCam> _logger = logger;

    private readonly IHostApplicationLifetime _applicationLifetime = applicationLifetime;
    
    private readonly RecyclableMemoryStreamManager _manager = new();

    public override AspectRatio ImageAspectRatio => new ((int)_capture.Get(CapProp.FrameWidth), (int)_capture.Get(CapProp.FrameHeight));

    public override void Connect()
    {
        _capture = new VideoCapture();
        _capture.Set(CapProp.FrameWidth, 1200);
        _capture.Set(CapProp.FrameHeight, 800);
        _logger.LogInformation("[WebCam] has been connected.");
    }

    public override void Disconnect()
    {
        _capture?.Dispose();
        _capture = null;
        _logger.LogInformation("[WebCam] has been disconnected.");
    }

    public override void Dispose()
    {
        _capture?.Dispose();
        _capture = null;
        _logger.LogInformation("[WebCam] has been disposed.");
    }

    public override void StartStream()
    {
        LiveViewActive = true;
        Task.Run(() =>
        {
            using Mat frame = new();
            while (!_applicationLifetime.ApplicationStopping.IsCancellationRequested
                && LiveViewActive)
            {
                _capture?.Read(frame);

                byte[] data = frame.ToImage<Rgb, byte>().ToJpegData();

                OnNewStreamImage(new MemoryStream(data));
            }
        }, _applicationLifetime.ApplicationStopping);
        _logger.LogInformation("[WebCam] liveView started.");
    }

    public override void StopStream()
    {
        LiveViewActive = false;
        _logger.LogInformation("[WebCam] liveView stopped.");
    }

    public override async Task<Image<Rgb24>> TakePictureAsync()
    {
        using Mat frame = new();
        _capture?.Read(frame);

        _logger.LogInformation("[WebCam] picture has been taken.");

        await Task.CompletedTask;

        return Image.Load<Rgb24>(frame.ToImage<Rgb, byte>().ToJpegData());
    }

    public override void Focus()
    {
        _logger.LogInformation("[WebCam] has focused.");
    }

    public static bool Connected()
    {
        using VideoCapture capture = new();
        return capture.IsOpened;
    }
}
