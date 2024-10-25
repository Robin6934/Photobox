using EOSDigital.API;
using EOSDigital.SDK;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.Camera;
internal class CanonCamera(ILogger<CanonCamera> logger, IHostApplicationLifetime applicationLifetime) : CameraBase(logger)
{
    private readonly CanonAPI api = new();

    private EOSDigital.API.Camera camera = default!;

    private readonly System.Timers.Timer keepAliveTimer = new()
    {
        AutoReset = true,
        Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
    };

    private bool secondTick = false;

    private readonly ILogger<CanonCamera> logger = logger;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

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
        logger.LogDebug("Keepalive Timer Ticked " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        secondTick ^= true;
    }

    private void Camera_LiveViewUpdated(EOSDigital.API.Camera sender, Stream img)
    {
        if (LiveViewActive)
        {
            WrapStream wrapStream = new(img);
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

        if (camera is not null)
        {
            keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;

            camera.OpenSession();

            logger.LogInformation("[Canon] has been connected.");
        }
    }

    public override void Disconnect()
    {
        camera.CloseSession();
        logger.LogInformation("[Canon] has been disconnected.");
    }

    public override void Dispose()
    {
        camera.Dispose();
        logger.LogInformation("[Canon] has been disposed.");
    }

    public override void Focus()
    {
        camera.SendCommand(CameraCommand.DoEvfAf);
        logger.LogInformation("[Canon] has focused.");
    }

    public override void StartStream()
    {
        LiveViewActive = true;
        camera.LiveViewUpdated += Camera_LiveViewUpdated;
        camera.StartLiveView();
        logger.LogInformation("[Canon] liveView started.");
    }

    public override void StopStream()
    {
        LiveViewActive = false;
        camera.LiveViewUpdated -= Camera_LiveViewUpdated;
        camera.StopLiveView();
        logger.LogInformation("[Canon] liveView stopped.");
    }

    public override Image<Rgb24> TakePicture()
    {
        TaskCompletionSource<DownloadInfo> tcs = new();

        string imagePath = ".\\Temp\\Temp.jpg";

        camera.TakePhoto();

        Folders.CheckIfDirectoriesExistElseCreate();

        camera.DownloadReady += (s, i) => tcs.SetResult(i);

        DownloadInfo info = tcs.Task.Result;

        logger.LogInformation("[Canon] picture has been taken under path {path}", imagePath);

        return Image.Load<Rgb24>(camera.DownloadToStream(info));
    }

    public static bool Connected()
    {
        CanonAPI api = new();
        return api.GetCameraList().Count != 0;
    }
}
