using EOSDigital.API;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Photobox.Lib.Camera;
public class CameraFactory(ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime)
{
    private readonly ILoggerFactory factory = loggerFactory;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;
    public ICamera Create()
    {
        CanonAPI api = new();
        if (api.GetCameraList().Count != 0)
        {
            return new CanonCamera(factory.CreateLogger<CanonCamera>(), applicationLifetime);
        }
        return new WebCam(factory.CreateLogger<WebCam>(), applicationLifetime);
    }
}
