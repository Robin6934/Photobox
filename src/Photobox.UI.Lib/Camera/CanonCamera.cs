using EOSDigital.API;
using EOSDigital.SDK;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rational = EOSDigital.SDK.Rational;

namespace Photobox.UI.Lib.Camera;

internal class CanonCamera : CameraBase
{
    private readonly CanonAPI api = new();

    private EOSDigital.API.Camera camera = default!;

    private readonly System.Timers.Timer keepAliveTimer = new()
    {
        AutoReset = true,
        Interval = TimeSpan.FromMinutes(1).TotalMilliseconds,
    };

    private bool secondTick = false;

    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly ILogger<CanonCamera> _logger;

    public CanonCamera(ILogger<CanonCamera> logger, IHostApplicationLifetime applicationLifetime)
        : base(logger)
    {
        _logger = logger;
        this.applicationLifetime = applicationLifetime;

        ErrorHandler.SevereErrorHappened += ErrorHandlerOnSevereErrorHappened;

        ErrorHandler.NonSevereErrorHappened += ErrorHandlerOnNonSevereErrorHappened;
    }

    private void ErrorHandlerOnNonSevereErrorHappened(object sender, ErrorCode errorCode)
    {
        _logger.LogWarning(
            "A non severe error happened in the canon camera error code: {errorCode}",
            errorCode
        );
        if (errorCode == ErrorCode.DEVICE_BUSY)
        {
            _logger.LogError("The canon camera is busy, could not connect.");
            throw new CameraNotFoundException("The canon camera is busy, could not connect.");
        }
        if (errorCode == ErrorCode.TAKE_PICTURE_AF_NG)
        {
            //camera.SendCommand(CameraCommand.PressShutterButton, 0);
        }
    }

    private void ErrorHandlerOnSevereErrorHappened(object sender, Exception ex)
    {
        _logger.LogError(ex, "An exception happened in the canon camera.");
        if (ex is SDKException { Error: ErrorCode.COMM_DISCONNECTED })
        {
            _logger.LogError("The camera got disconnected.");
        }

        throw ex;
    }

    public override AspectRatio ImageAspectRatio => new(1056, 704);

    private void KeepAliveTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (secondTick)
        {
            camera.SendCommand(CameraCommand.DriveLensEvf, (int)DriveLens.Near1);
        }
        else
        {
            camera.SendCommand(CameraCommand.DriveLensEvf, (int)DriveLens.Far1);
        }
        _logger.LogDebug(
            "Keepalive Timer Ticked " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
        );
        secondTick ^= true;
    }

    private void Camera_LiveViewUpdated(EOSDigital.API.Camera sender, Stream img)
    {
        if (LiveViewActive)
        {
            using WrapStream wrapStream = new(img);
            OnNewStreamImage(wrapStream);
        }
    }

    public override void Connect()
    {
        List<EOSDigital.API.Camera> CamList = api.GetCameraList();

        if (CamList.Count > 0)
        {
            camera = CamList.FirstOrDefault()!;
        }
        else
        {
            throw new CameraNotFoundException("No Canon camera has been found.");
        }

        keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;

        keepAliveTimer.Start();

        camera.OpenSession();

        camera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Both);

        camera.SetCapacity(4096, int.MaxValue);

        _logger.LogInformation("[Canon] has been connected.");
    }

    public override void Focus()
    {
        camera.SendCommand(CameraCommand.DoEvfAf, (int)EvfAFMode.Live);

        _logger.LogInformation("[Canon] has focused.");
    }

    public override void StartStream()
    {
        if (!LiveViewActive)
        {
            LiveViewActive = true;
            camera.LiveViewUpdated += Camera_LiveViewUpdated;
            camera.StartLiveView();
            _logger.LogInformation("[Canon] liveView started.");
        }
    }

    public override void StopStream()
    {
        if (LiveViewActive)
        {
            LiveViewActive = false;
            camera.LiveViewUpdated -= Camera_LiveViewUpdated;
            camera.StopLiveView();
            _logger.LogInformation("[Canon] liveView stopped.");
        }
    }

    public override async Task<Image<Rgb24>> TakePictureAsync()
    {
        camera.SendCommand(CameraCommand.DoEvfAf, (int)EvfAFMode.Quick);

        TaskCompletionSource<DownloadInfo> tcs = new();

        camera.DownloadReady += HandleDownloadReady;

        try
        {
            await camera.TakePhotoAsync();
            //camera.SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.Completely);
            //camera.SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.OFF);

            DownloadInfo info = await tcs.Task;

            return Image.Load<Rgb24>(camera.DownloadToStream(info));
        }
        finally
        {
            camera.DownloadReady -= HandleDownloadReady;
        }

        void HandleDownloadReady(object? sender, DownloadInfo info)
        {
            if (!tcs.Task.IsCompleted)
            {
                tcs.SetResult(info);
            }
        }
    }

    public override void Disconnect()
    {
        camera.CloseSession();
        _logger.LogInformation("[Canon] has been disconnected.");
    }

    public override void Dispose()
    {
        StopStream();
        camera.Dispose();
        api.Dispose();
        keepAliveTimer.Stop();
        _logger.LogInformation("[Canon] has been disposed.");
    }

    public static bool Connected()
    {
        CanonAPI api = new();
        return api.GetCameraList().Count != 0;
    }
}
