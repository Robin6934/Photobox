using EOSDigital.API;
using EOSDigital.SDK;
using Microsoft.Extensions.Logging;

namespace Photobox.Lib.Camera;
internal class CanonCamera : CameraBase
{
    private readonly CanonAPI api = new();

    private readonly EOSDigital.API.Camera camera;

    private readonly System.Timers.Timer keepAliveTimer = new()
    {
        AutoReset = true,
        Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
    };

    private bool secondTick = false;

    private readonly ILogger<CanonCamera> logger;

    public CanonCamera(ILogger<CanonCamera> logger)
    {
        this.logger = logger;

        int cnt = 0;
        bool CameraFound = false;
        List<EOSDigital.API.Camera> CamList = [];

        while (!CameraFound)
        {
            try { CamList = api.GetCameraList(); }
            catch (Exception) { throw new CameraException("No Canon camera is connected!"); }
            if (CamList.Count > 0)
            {
                camera = CamList[0];
                CameraFound = true;
            }
            if (cnt > 10)
            {
                break;
            }
            Task.Delay(100).Wait();
            cnt++;
        }

        if (CameraFound)
        {
            camera.LiveViewUpdated += Camera_LiveViewUpdated;

            keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;
        }
    }

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

    public override Task ConnectAsync()
    {
        camera.OpenSession();
        return Task.CompletedTask;
    }

    public override Task DisconnectAsync()
    {
        camera.CloseSession();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        camera.Dispose();
    }

    public override Task FocusAsync()
    {
        camera.SendCommand(CameraCommand.DoEvfAf);
        return Task.CompletedTask;
    }

    public override Task StartStreamAsync()
    {
        camera.StartLiveView();
        return Task.CompletedTask;
    }

    public override Task StopStreamAsync()
    {
        FocusAsync();
        camera.StopLiveView();
        return Task.CompletedTask;
    }

    public override async Task<string> TakePictureAsync()
    {
        TaskCompletionSource<DownloadInfo> tcs = new();

        string imagePath = Folders.NewImagePath;

        await camera.TakePhotoAsync();

        Folders.CheckIfDirectoriesExistElseCreate();

        camera.DownloadReady += (s, i) => tcs.SetResult(i);

        DownloadInfo info = await tcs.Task;

        camera.DownloadFile(info, imagePath);

        return imagePath;
    }
}
