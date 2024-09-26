using Microsoft.AspNetCore.Mvc;

namespace Photobox.LocalServer.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ApplicationController(IHostApplicationLifetime lifetime) : ControllerBase
{
    private readonly IHostApplicationLifetime lifetime = lifetime;

    [HttpGet]
    public async Task<IActionResult> ShutDown()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            lifetime.StopApplication();
        });

        await Task.CompletedTask;

        return Ok("The application has been shutdown successfully!");
    }
}
