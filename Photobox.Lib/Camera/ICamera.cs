namespace Photobox.Lib.Camera;

public interface ICamera : IDisposable
{
    event NewImageHandler CameraStream;

    event NewImageHandler PictureTaken;

    Task ResilientConnectAsync();
    Task ConnectAsync();
    Task StartStreamAsync();
    Task StopStreamAsync();
    Task FocusAsync();
    Task TakePictureAsync();
    Task DisconnectAsync();
}
