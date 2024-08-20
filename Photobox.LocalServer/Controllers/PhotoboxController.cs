using Microsoft.AspNetCore.Mvc;
using Photobox.Lib;
using Photobox.Lib.PhotoManager;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoboxController(IImageManager imageManager) : Controller
{
    private readonly IImageManager imageManager = imageManager;

    private static readonly string[] allowedExtensions =
        [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".ico", ".webp"];

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
        IActionResult result = CheckImage(imagePath);

        if (!allowedExtensions.Contains(Path.GetExtension(imagePath).ToLower()))
        {
            result = StatusCode(StatusCodes.Status415UnsupportedMediaType, "The File type is not supported");
        }

        await imageManager.PrintAndSaveAsync(imagePath);

        return result;
    }

    /// <summary>
    /// Handles saving an image from the specified path.
    /// </summary>
    /// <param name="imagePath">The path of the image to print.</param>
    /// <returns>
    /// Returns a 200 OK if the image is found and can be saved, otherwise returns a 404 Not Found.
    /// </returns>
    /// <response code="200">The image was found and is being printed.</response>
    /// <response code="404">The image path was not found.</response>
    /// <response code="403">The image path is not located in the photobox folder</response>
    [HttpGet("{imagePath}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Save(string imagePath)
    {
        IActionResult result = CheckImage(imagePath);

        imageManager.Save(imagePath);

        return result;
    }

    /// <summary>
    /// Handles deleting an image from the specified path.
    /// </summary>
    /// <param name="imagePath">The path of the image to print.</param>
    /// <returns>
    /// Returns a 200 OK if the image is found and can be saved, otherwise returns a 404 Not Found.
    /// </returns>
    /// <response code="200">The image was found and is being printed.</response>
    /// <response code="404">The image path was not found.</response>
    /// <response code="403">The image path is not located in the photobox folder</response>
    [HttpGet("{imagePath}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(string imagePath)
    {
        IActionResult result = CheckImage(imagePath);

        imageManager.Delete(imagePath);

        return result;
    }

    private IActionResult CheckImage(string imagePath)
    {
        FileInfo info = new(imagePath);

        string normalizedPhotoboxDirectory = Folders.PhotoboxBaseDir.TrimEnd(Path.DirectorySeparatorChar);

        string normalizedImagePath = Path.GetFullPath(imagePath).TrimEnd(Path.DirectorySeparatorChar);

        if (!info.Exists)
        {
            return NotFound("Image path not found.");
        }

        if (!normalizedImagePath.Contains(normalizedPhotoboxDirectory))
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Access to the specified folder is forbidden.");
        }

        return Ok("Image is being printed.");
    }


}
