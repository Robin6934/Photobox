using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CameraController(ICamera camera, IIPCServer ipcServer) : ControllerBase
{
    private readonly ICamera camera = camera;

    private readonly IIPCServer ipcServer = ipcServer;

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
}
