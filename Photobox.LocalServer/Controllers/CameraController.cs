using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Photobox.LocalServer.Models;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CameraController(ICamera camera,
    IIPCServer ipcServer,
    ILogger<CameraController> logger,
    IHostApplicationLifetime applicationLifetime) : ControllerBase
{
    private readonly ICamera camera = camera;

    private readonly IIPCServer ipcServer = ipcServer;

    private readonly ILogger<CameraController> logger = logger;

    private static bool isStreamStarted = false;

    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        if (applicationLifetime.ApplicationStopping.IsCancellationRequested)
        {
            logger.LogInformation("The application is already shutting down so no stream could be started");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The server is shutting down");
        }

        if (isStreamStarted)
        {
            logger.LogInformation("The camera stream has already been started");
            return StatusCode(StatusCodes.Status208AlreadyReported, "The camera stream has already been started");
        }

        await camera.ConnectAsync();
        _ = ipcServer.ConnectAsync();

        camera.CameraStream += async (o, s) => await ipcServer.SendAsync(s);

        _ = camera.StartStreamAsync();
        isStreamStarted = true;

        logger.LogInformation("Camera stream started succesfully");
        return Ok("Camera stream started.");
    }

    [HttpGet]
    public async Task<IActionResult> Stop()
    {
        if (isStreamStarted)
        {
            ipcServer.Disconnect();
            await camera.StopStreamAsync();
            isStreamStarted = false;
        }
        logger.LogInformation("Camera stream stopped succesfully");
        return Ok("Camera stream stopped.");
    }

    [HttpGet]
    public async Task<TakePictureResultModel> TakePicture()
    {
        string imagePath = await camera.TakePictureAsync();

        var result = new TakePictureResultModel
        {
            ImagePath = imagePath
        };

        logger.LogInformation("New picture taken and stored under path: {imagePath}", imagePath);

        return result;
    }


}
