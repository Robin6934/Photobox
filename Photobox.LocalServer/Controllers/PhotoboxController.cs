using Microsoft.AspNetCore.Mvc;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoboxController : Controller
{
    /// <summary>
    /// Handles printing an image from the specified path.
    /// </summary>
    /// <param name="imagePath">The path of the image to print.</param>
    /// <returns>
    /// Returns a 200 OK if the image is found and can be printed, otherwise returns a 404 Not Found.
    /// </returns>
    /// <response code="200">The image was found and is being printed.</response>
    /// <response code="404">The image path was not found.</response>
    [HttpGet("{imagePath}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Print(string imagePath)
    {
        if (System.IO.File.Exists(imagePath))
        {
            // Logic to print the image goes here

            return Ok("Image is being printed.");
        }

        return NotFound("Image path not found.");
    }

}
