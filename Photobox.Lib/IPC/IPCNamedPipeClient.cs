using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Camera;
using Photobox.LocalServer.RestApi.Api;
using System.Diagnostics;
using System.IO.Pipes;

namespace Photobox.Lib.IPC
{
    public class IPCNamedPipeClient(
        ILogger<IPCNamedPipeClient> logger,
        ICameraApi cameraApi,
        IHostApplicationLifetime applicationLifetime) : CameraBase
    {
        private NamedPipeClientStream pipeClientStream = null!;

        ILogger<IPCNamedPipeClient> logger = logger;

        private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

        private readonly ICameraApi cameraApi = cameraApi;

        public override async Task ConnectAsync()
        {
            pipeClientStream = new NamedPipeClientStream(".", "Camera_Stream", PipeDirection.InOut, PipeOptions.Asynchronous);

            logger.LogDebug("Starting wait for named pipe connection client");

            Task startTask = cameraApi.ApiCameraStartGetAsync();

            // Connect to the named pipe
            Task pipeConnectionTask = pipeClientStream.ConnectAsync();

            await startTask;

            logger.LogDebug("HTTP request successful");
            // Await the pipe connection task
            await pipeConnectionTask;
            Debug.WriteLine("Finished wait for named pipe connection client");

            logger.LogDebug("Connected to pipe.");
        }

        public override async Task DisconnectAsync()
        {
            pipeClientStream?.Close();
            pipeClientStream?.Dispose();
            await Task.CompletedTask;
        }

        public override async Task StartStreamAsync()
        {
            LiveViewActive = true;
            try
            {
                while (!applicationLifetime.ApplicationStopping.IsCancellationRequested
                    && LiveViewActive)
                {
                    byte[] lengthBuffer = new byte[sizeof(int)];
                    int bytesRead = await pipeClientStream.ReadAsync(
                        lengthBuffer.AsMemory(0, sizeof(int)),
                        applicationLifetime.ApplicationStopping);

                    if (bytesRead == 0)
                    {
                        logger.LogError("End of stream or no data received.");
                        break; // Exit if no data is received
                    }

                    int length = BitConverter.ToInt32(lengthBuffer, 0);

                    if (length <= 0)
                    {
                        logger.LogError("Received invalid length.");
                        continue; // Skip if length is invalid
                    }

                    byte[] dataBuffer = new byte[length];
                    bytesRead = await pipeClientStream.ReadAsync(
                        dataBuffer.AsMemory(0, length),
                        applicationLifetime.ApplicationStopping);

                    if (bytesRead == 0)
                    {
                        logger.LogError("End of stream or no data received.");
                        break; // Exit if no data is received
                    }

                    // Validate received data
                    if (dataBuffer.Length != length)
                    {
                        logger.LogError("Data length mismatch.");
                        continue; // Skip if data length is incorrect
                    }

                    try
                    {
                        MemoryStream ms = new(dataBuffer);

                        ms.Seek(0, SeekOrigin.Begin);

                        OnNewStreamImage(ms);
                    }
                    catch (ArgumentException ex)
                    {
                        logger.LogError($"Error creating Bitmap: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in streaming task: {ex.Message}");
            }
        }

        public override async Task StopStreamAsync()
        {
            LiveViewActive = false;

            await cameraApi.ApiCameraStopGetAsync();
        }


        public override async Task FocusAsync()
        {
            await Task.Delay(1000);
        }

        public override async Task<string> TakePictureAsync()
        {
            var result = await cameraApi.ApiCameraTakePictureGetAsync();

            return result.ImagePath;
        }

        public override void Dispose()
        {

        }
    }
}
