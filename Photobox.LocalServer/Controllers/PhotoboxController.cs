using Microsoft.AspNetCore.Mvc;
using Photobox.Lib;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoboxController : Controller
{
    private static readonly string[] allowedExtensions = { "jpg", "jpeg" };

    /// <summary>
    /// Handles printing an image from the specified path.
    /// </summary>
    /// <param name="imagePath">The path of the image to print.</param>
    /// <returns>
    /// Returns a 200 OK if the image is found and can be printed, otherwise returns a 404 Not Found.
    /// </returns>
    /// <response code="200">The image was found and is being printed.</response>
    /// <response code="404">The image path was not found.</response>
    /// <response code="403">The image path is not located in the photobox folder</response>
    /// <response code="415">The image path is not an image</response>
    [HttpGet("{imagePath}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> Print(string imagePath)
    {
        IActionResult result = Ok("Image is being printed.");
        FileInfo info = new(imagePath);

        string normalizedPhotoboxDirectory = Folders.PhotoBoothBaseDir.TrimEnd(Path.DirectorySeparatorChar);

        string normalizedImagePath = Path.GetFullPath(imagePath).TrimEnd(Path.DirectorySeparatorChar);
        
        if (!info.Exists)
        {
            result = NotFound("Image path not found.");
        }

        if (!normalizedImagePath.Contains(normalizedPhotoboxDirectory))
        {
            result = StatusCode(StatusCodes.Status403Forbidden, "Access to the specified folder is forbidden.");
        }

        if (!allowedExtensions.Contains(info.Extension))
        {
            result = StatusCode(StatusCodes.Status415UnsupportedMediaType, "The File type is not supported");
        }



        return result;
    }

}
