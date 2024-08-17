namespace Photobox.Lib.Camera;

public interface ICamera : IDisposable
{
    event NewImageHandler CameraStream;
    Task ResilientConnectAsync();
    Task ConnectAsync();
    Task StartStreamAsync();
    Task StopStreamAsync();
    Task FocusAsync();
    Task<string> TakePictureAsync();
    Task DisconnectAsync();
}
