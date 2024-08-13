using Photobox.Lib.Camera;
using Photobox.Lib.Models;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Net.Http.Json;

namespace Photobox.Lib.IPC
{
    public class IPCNamedPipeClient : CameraBase
    {
        private NamedPipeClientStream pipeClientStream = null!;

        private CancellationTokenSource cts = new();

        private HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:7176"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        public override async Task ConnectAsync()
        {
            pipeClientStream = new NamedPipeClientStream(".", "Camera_Stream", PipeDirection.InOut, PipeOptions.Asynchronous);

            Debug.WriteLine("Starting wait for named pipe connection client");

            Task<HttpResponseMessage> task1 = httpClient.GetAsync("/api/Camera/Start");

            // Connect to the named pipe
            Task pipeConnectionTask = pipeClientStream.ConnectAsync();

            HttpResponseMessage i = await task1;
            Debug.WriteLine("HTTP request successful");
            // Await the pipe connection task
            await pipeConnectionTask;
            Debug.WriteLine("Finished wait for named pipe connection client");

            Console.WriteLine("Connected to pipe.");
        }



        public override async Task DisconnectAsync()
        {
            pipeClientStream?.Close();
            pipeClientStream?.Dispose();
        }

        public override async Task StartStreamAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        byte[] lengthBuffer = new byte[sizeof(int)];
                        int bytesRead = await pipeClientStream.ReadAsync(lengthBuffer.AsMemory(0, sizeof(int)), cts.Token);

                        if (bytesRead == 0)
                        {
                            Debug.WriteLine("End of stream or no data received.");
                            break; // Exit if no data is received
                        }

                        int length = BitConverter.ToInt32(lengthBuffer, 0);

                        if (length <= 0)
                        {
                            Debug.WriteLine("Received invalid length.");
                            continue; // Skip if length is invalid
                        }

                        byte[] dataBuffer = new byte[length];
                        bytesRead = await pipeClientStream.ReadAsync(dataBuffer.AsMemory(0, length), cts.Token);

                        if (bytesRead == 0)
                        {
                            Debug.WriteLine("End of stream or no data received.");
                            break; // Exit if no data is received
                        }

                        // Validate received data
                        if (dataBuffer.Length != length)
                        {
                            Debug.WriteLine("Data length mismatch.");
                            continue; // Skip if data length is incorrect
                        }

                        try
                        {
                            using MemoryStream ms = new(dataBuffer);
                            ms.Seek(0, SeekOrigin.Begin);

                            Bitmap bitmap = new(ms);
                            OnNewStreamImage(bitmap);
                        }
                        catch (ArgumentException ex)
                        {
                            Debug.WriteLine($"Error creating Bitmap: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in streaming task: {ex.Message}");
                }
            }, cts.Token);
        }

        public override async Task StopStreamAsync()
        {
            cts.Cancel();

            await httpClient.GetAsync("/api/Camera/Stop");
        }


        public override async Task FocusAsync()
        {
            await Task.Delay(1000);
        }

        public override async Task<string> TakePictureAsync()
        {
            var result = await httpClient.GetAsync("api/Camera/TakePicture");
            var resultModel = await result.Content
                .ReadFromJsonAsync<TakePictureResultModel>();

            return resultModel.ImagePath;
        }

        public override void Dispose()
        {

        }
    }
}
