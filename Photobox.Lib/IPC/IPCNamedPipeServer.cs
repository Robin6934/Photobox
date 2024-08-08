using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;

namespace Photobox.Lib.IPC
{
    public class IPCNamedPipeServer : IIPCServer
    {
        private NamedPipeServerStream pipeServerStream = null!;

        private readonly CancellationTokenSource cts = new();

        public async Task ConnectAsync()
        {
            pipeServerStream = NamedPipeServerStreamAcl.Create("Camera_Stream",
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                1024 * 1024 * 100,
                1024 * 1024 * 100,
                null) ?? throw new Exception("Failed to create named pipe");

            Debug.WriteLine("Starting wait for named pipe connection Server");

            await pipeServerStream.WaitForConnectionAsync();

            Debug.WriteLine("finished wait for named pipe connection Server");
        }

        public void Disconnect()
        {
            cts.Cancel();
            pipeServerStream.Close();
            pipeServerStream.Dispose();
        }

        public async Task SendAsync(Bitmap bitmap)
        {
            if (!cts.Token.IsCancellationRequested)
            {
                byte[] frameBytes = BitmapToBytes(bitmap);
                bitmap.Dispose();

                await pipeServerStream.WriteAsync(BitConverter.GetBytes(frameBytes.Length));
                await pipeServerStream.WriteAsync(frameBytes);
                await pipeServerStream.FlushAsync();
            }
        }

        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            using MemoryStream stream = new();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            return stream.ToArray();
        }
    }
}
