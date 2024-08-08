using System.Drawing;

namespace Photobox.Lib.IPC
{
    public interface IIPCServer
    {
        Task ConnectAsync();

        void Disconnect();

        Task SendAsync(Bitmap bitmap);
    }
}
