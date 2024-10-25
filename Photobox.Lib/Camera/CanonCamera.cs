using EOSDigital.API;
using EOSDigital.SDK;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Photobox.Lib.Camera;
internal class CanonCamera(ILogger<CanonCamera> logger, IHostApplicationLifetime applicationLifetime) : CameraBase
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
        WrapStream wrapStream = new(img);
        OnNewStreamImage(wrapStream);
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
            camera.LiveViewUpdated += Camera_LiveViewUpdated;

            keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;

            camera.OpenSession();
        }
    }

    public override void Disconnect()
    {
        camera.CloseSession();
    }

    public override void Dispose()
    {
        camera.Dispose();
    }

    public override void Focus()
    {
        camera.SendCommand(CameraCommand.DoEvfAf);
    }

    public override void StartStream()
    {
        camera.StartLiveView();
        LiveViewActive = true;
    }

    public override void StopStream()
    {
        camera.StopLiveView();
        LiveViewActive = false;
    }

    public override string TakePicture()
    {
        TaskCompletionSource<DownloadInfo> tcs = new();

        string imagePath = Folders.NewImagePath;

        camera.TakePhoto();

        Folders.CheckIfDirectoriesExistElseCreate();

        camera.DownloadReady += (s, i) => tcs.SetResult(i);

        DownloadInfo info = tcs.Task.Result;

        camera.DownloadFile(info, imagePath);

        return imagePath;
    }

    public static bool Connected()
    {
        CanonAPI api = new();
        return api.GetCameraList().Count != 0;
    }
}
