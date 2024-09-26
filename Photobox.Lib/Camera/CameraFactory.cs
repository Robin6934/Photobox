using Microsoft.Extensions.Logging;

namespace Photobox.Lib.Camera;
public class CameraFactory(ILoggerFactory loggerFactory)
{
    private readonly ILoggerFactory factory = loggerFactory;
    public ICamera Create()
    {
        return new CanonCamera(factory.CreateLogger<CanonCamera>());
    }
}
