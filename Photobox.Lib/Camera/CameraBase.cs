using EOSDigital.API;
using Polly;
using Polly.CircuitBreaker;
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
            MaxRetryAttempts = 5,
            ShouldHandle = args => args.Outcome switch
            {
                { Exception: CameraNotFoundException } => PredicateResult.True(),
                _ => PredicateResult.False()
            }
        })
        .Build();

    public virtual void ResilientConnect() => pipeline.Execute(Connect);

    public abstract void Connect();

    public abstract void StartStream();

    public abstract void StopStream();

    public abstract void Focus();

    public abstract string TakePicture();

    public abstract void Disconnect();

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
