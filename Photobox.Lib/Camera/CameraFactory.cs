using System.Runtime.InteropServices;

namespace Photobox.Lib.Camera;
public class CameraFactory
{
    public ICamera Create()
    {
        return new WebCam();
    }
}
