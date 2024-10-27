using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Lib.Camera;

public interface ICamera : IDisposable
{
    event NewImageHandler CameraStream;

    public bool LiveViewActive { get; }
    void ResilientConnect();
    void Connect();
    void StartStream();
    void StopStream();
    void Focus();
    Task<Image<Rgb24>> TakePictureAsync();
    void Disconnect();
}
