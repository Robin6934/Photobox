using Polly;
using Polly.Retry;
using Polly.Timeout;
using System.Drawing;

namespace Photobox.Lib.Camera;

public delegate void NewImageHandler(object sender, Bitmap img);

public abstract class CameraBase : ICamera
{
    public event NewImageHandler CameraStream = default!;

    public event NewImageHandler PictureTaken = default!;

    private readonly ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
        .AddTimeout(new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(5)
        })
        .AddRetry(new RetryStrategyOptions
        {
            Delay = TimeSpan.FromSeconds(1),
            MaxRetryAttempts = 3
        })
        .Build();

    public virtual Task ResilientConnectAsync() => pipeline.Execute(ConnectAsync);


    public abstract Task ConnectAsync();

    public abstract Task StartStreamAsync();

    public abstract Task StopStreamAsync();

    public abstract Task FocusAsync();

    public abstract Task<string> TakePictureAsync();

    public abstract Task DisconnectAsync();

    protected virtual void OnNewStreamImage(Bitmap img)
    {
        CameraStream?.Invoke(this, img);
    }

    protected virtual void OnPictureTaken(Bitmap img)
    {
        PictureTaken?.Invoke(this, img);
    }

    public abstract void Dispose();
}
