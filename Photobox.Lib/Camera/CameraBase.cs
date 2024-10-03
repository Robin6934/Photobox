using Polly;
using Polly.Retry;
using Polly.Timeout;
using System.Drawing;

namespace Photobox.Lib.Camera;

public delegate void NewImageHandler(object sender, Stream img);

public abstract class CameraBase : ICamera
{
    public event NewImageHandler CameraStream = default!;

    public virtual bool LiveViewActive { get; set; } = false;

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
        MemoryStream stream = new();

        img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

        stream.Seek(0, SeekOrigin.Begin);

        CameraStream?.Invoke(this, stream);
    }

    protected virtual void OnNewStreamImage(Stream stream)
    {
        CameraStream?.Invoke(this, stream);
    }

    public abstract void Dispose();
}
