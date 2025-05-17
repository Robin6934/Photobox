using EOSDigital.API;
using EOSDigital.SDK;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rational = EOSDigital.SDK.Rational;

namespace Photobox.UI.Lib.Camera;

internal class CanonCamera(
    ILogger<CanonCamera> logger,
    IHostApplicationLifetime applicationLifetime
) : CameraBase(logger)
{
    private readonly CanonAPI api = new();

    private EOSDigital.API.Camera camera = default!;

    private readonly System.Timers.Timer keepAliveTimer = new()
    {
        AutoReset = true,
        Interval = TimeSpan.FromMinutes(1).TotalMilliseconds,
    };

    private bool secondTick = false;

    private readonly ILogger<CanonCamera> logger = logger;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

    public override AspectRatio ImageAspectRatio => new(960, 540);

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
        logger.LogDebug(
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

        logger.LogInformation("[Canon] has been connected.");
    }

    public override void Focus()
    {
        camera.SendCommand(CameraCommand.DoEvfAf, (int)EvfAFMode.Live);

        logger.LogInformation("[Canon] has focused.");
    }

    public override void StartStream()
    {
        if (!LiveViewActive)
        {
            LiveViewActive = true;
            camera.LiveViewUpdated += Camera_LiveViewUpdated;
            camera.StartLiveView();
            logger.LogInformation("[Canon] liveView started.");
        }
    }

    public override void StopStream()
    {
        if (LiveViewActive)
        {
            LiveViewActive = false;
            camera.LiveViewUpdated -= Camera_LiveViewUpdated;
            camera.StopLiveView();
            logger.LogInformation("[Canon] liveView stopped.");
        }
    }

    public override async Task<Image<Rgb24>> TakePictureAsync()
    {
        TaskCompletionSource<DownloadInfo> tcs = new();

        camera.DownloadReady += HandleDownloadReady;

        try
        {
            camera.TakePhoto();

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
        logger.LogInformation("[Canon] has been disconnected.");
    }

    public override void Dispose()
    {
        StopStream();
        camera.Dispose();
        api.Dispose();
        keepAliveTimer.Stop();
        logger.LogInformation("[Canon] has been disposed.");
    }

    public static bool Connected()
    {
        CanonAPI api = new();
        return api.GetCameraList().Count != 0;
    }
}
