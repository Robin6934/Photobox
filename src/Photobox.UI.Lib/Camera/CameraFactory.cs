using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Photobox.UI.Lib.Camera;

public class CameraFactory(
    ILogger<CameraFactory> logger,
    ILoggerFactory loggerFactory,
    IHostApplicationLifetime applicationLifetime
)
{
    private readonly ILoggerFactory factory = loggerFactory;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

    private readonly ILogger<CameraFactory> logger = logger;

    public ICamera Create()
    {
        if (CanonCamera.Connected())
        {
            return new CanonCamera(factory.CreateLogger<CanonCamera>(), applicationLifetime);
        }
        if (WebCam.Connected())
        {
            return new WebCam(factory.CreateLogger<WebCam>(), applicationLifetime);
        }

        logger.LogCritical("No Camera is Connected! Application is shutting down...");
        applicationLifetime.StopApplication();
        throw new InvalidOperationException("Failed to create a camera: No camera is connected.");
    }

    public ICamera Create(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.Auto:
                return Create();
            case CameraType.WebCam:
                return new WebCam(factory.CreateLogger<WebCam>(), applicationLifetime);
            case CameraType.Canon:
                return new CanonCamera(factory.CreateLogger<CanonCamera>(), applicationLifetime);
            default:
                throw new InvalidOperationException("The Cameratype is not supported!");
        }
    }
}
