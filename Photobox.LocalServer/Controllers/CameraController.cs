using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Photobox.LocalServer.Models;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CameraController(ICamera camera, IIPCServer ipcServer, ILogger<CameraController> logger) : ControllerBase
{
    private readonly ICamera camera = camera;

    private readonly IIPCServer ipcServer = ipcServer;

    private readonly ILogger<CameraController> logger = logger;

    private static bool isStreamStarted = false;

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        if (!isStreamStarted)
        {
            await camera.ConnectAsync();
            _ = ipcServer.ConnectAsync();

            camera.CameraStream += async (o, s) => await ipcServer.SendAsync(s);

            _ = camera.StartStreamAsync();
            isStreamStarted = true;
        }
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

        logger.LogInformation("New picture taken and stored under {imagePath}", imagePath);

        return result;
    }


}
