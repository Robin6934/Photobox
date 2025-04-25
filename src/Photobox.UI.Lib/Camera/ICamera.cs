using Photobox.Lib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Camera;

public interface ICamera : IDisposable
{
    event NewImageHandler CameraStream;

    public bool LiveViewActive { get; }

    public AspectRatio ImageAspectRatio { get; }

    void ResilientConnect();

    void Connect();

    void StartStream();

    void StopStream();

    void Focus();

    Task<Image<Rgb24>> TakePictureAsync();

    void Disconnect();
}
