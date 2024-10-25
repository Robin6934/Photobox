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
    string TakePicture();
    void Disconnect();
}
