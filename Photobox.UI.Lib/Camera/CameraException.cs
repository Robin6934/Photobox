namespace Photobox.UI.Lib.Camera;

[Serializable]
public class CameraException : Exception
{
    public CameraException() { }

    public CameraException(string message)
        : base(message) { }

    public CameraException(string message, Exception inner)
        : base(message, inner) { }
}
