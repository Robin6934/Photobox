using Microsoft.Extensions.Logging;
using Photobox.Lib;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Camera;

public delegate void NewImageHandler(object sender, Stream img);

public abstract class CameraBase(ILogger logger) : ICamera
{
    public event NewImageHandler CameraStream = default!;

    private readonly ILogger logger = logger;

    public virtual bool LiveViewActive { get; set; } = false;

    public virtual AspectRatio ImageAspectRatio { get; }

    private readonly ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
        .AddTimeout(new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(5)
        })
        .AddRetry(new RetryStrategyOptions
        {
            Delay = TimeSpan.FromSeconds(1),
            MaxRetryAttempts = 5,
            ShouldHandle = args =>
            {
                if (args.Outcome.Exception is CameraNotFoundException ex)
                {
                    // Add logging here
                    logger.LogWarning("Retrying due to CameraNotFoundException: {Message}", ex.Message);

                    // Return PredicateResult.True() to retry
                    return PredicateResult.True();
                }
                return PredicateResult.False();
            }

        })
        .Build();

    public virtual void ResilientConnect() => pipeline.Execute(Connect);

    public abstract void Connect();

    public abstract void StartStream();

    public abstract void StopStream();

    public abstract void Focus();

    public abstract Task<Image<Rgb24>> TakePictureAsync();

    public abstract void Disconnect();

    protected virtual void OnNewStreamImage(Stream stream)
    {
        CameraStream?.Invoke(this, stream);
        stream.Dispose();
    }

    public abstract void Dispose();
}
