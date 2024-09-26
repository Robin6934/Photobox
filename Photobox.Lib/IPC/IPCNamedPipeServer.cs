using System.Diagnostics;
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

        public async Task SendAsync(Stream stream)
        {
            if (!cts.Token.IsCancellationRequested)
            {
                byte[] buffer = new byte[stream.Length]; // Adjust buffer size as needed

                int bytesRead = await stream.ReadAsync(buffer.AsMemory());

                // Send the length of the data (frameBytes.Length)
                await pipeServerStream.WriteAsync(BitConverter.GetBytes(bytesRead).AsMemory(0, sizeof(int)));

                // Send the actual data
                await pipeServerStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                await pipeServerStream.FlushAsync();

                await stream.DisposeAsync();
            }
        }
    }
}
