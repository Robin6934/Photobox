namespace Photobox.Lib.Camera;

public interface ICamera : IDisposable
{
    event NewImageHandler CameraStream;

    public bool LiveViewActive { get; }
    Task ResilientConnectAsync();
    Task ConnectAsync();
    Task StartStreamAsync();
    Task StopStreamAsync();
    Task FocusAsync();
    Task<string> TakePictureAsync();
    Task DisconnectAsync();
}
