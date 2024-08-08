using Microsoft.AspNetCore.Mvc;
using Photobox.Lib.Camera;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CameraController(ICamera camera) : Controller
{
    private readonly ICamera camera = camera;

    public async Task<IActionResult> StartCameraStream()
    {
        await camera.ConnectAsync();

        _ = camera.StartStreamAsync();

        return Ok();
    }

    public async Task<IActionResult> StopCameraStream()
    {
        await camera.StopStreamAsync();

        return Ok();
    }

}
