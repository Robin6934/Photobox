using Microsoft.AspNetCore.Mvc;
using Photobox.Lib;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Photobox.Lib.Models;

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
            await ipcServer.ConnectAsync();

            camera.CameraStream += async (s, i) => await ipcServer.SendAsync(i);

            _ = camera.StartStreamAsync();
            isStreamStarted = true;
        }
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
        return Ok("Camera stream stopped.");
    }

    [HttpGet]
    public async Task<IActionResult> TakePicture()
    {
        string imagePath = await camera.TakePictureAsync();

        var result = new TakePictureResultModel 
        {
            ImagePath = imagePath 
        };

        return Ok(result);
    }


}
