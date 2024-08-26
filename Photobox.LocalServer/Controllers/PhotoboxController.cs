using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Photobox.Lib;
using Photobox.Lib.PhotoManager;
using Photobox.LocalServer.ConfigModels;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public partial class PhotoboxController(
    ILogger<PhotoboxController> logger,
    IImageManager imageManager,
    IOptionsMonitor<PhotoboxConfig> options) : Controller
{
    private readonly ILogger<PhotoboxController> logger = logger;

    private readonly IImageManager imageManager = imageManager;

    private readonly PhotoboxConfig photoboxConfig = options.CurrentValue;

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
        var (success, actionResult) = CheckImage(imagePath);
        if (!success)
        {
            return actionResult;
        }

        if (!allowedExtensions.Contains(Path.GetExtension(imagePath).ToLower()))
        {
            logger.LogError("The file: {imagePath} is not supported", imagePath);
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, "The File type is not supported");
        }

        await imageManager.PrintAndSaveAsync(imagePath, photoboxConfig.PrinterName);

        return Ok("Image is being printed.");
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
    public IActionResult Save(string imagePath)
    {
        var (success, actionResult) = CheckImage(imagePath);
        if (!success)
        {
            return actionResult;
        }

        imageManager.Save(imagePath);

        return Ok("Image saved successfully.");
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
    public IActionResult Delete(string imagePath)
    {
        var (success, actionResult) = CheckImage(imagePath);
        if (!success)
        {
            return actionResult;
        }

        imageManager.Delete(imagePath);

        return Ok("Image deleted successfully.");
    }

    private (bool success, IActionResult actionResult) CheckImage(string imagePath)
    {
        string normalizedPhotoboxDirectory = Folders.PhotoboxBaseDir.TrimEnd(Path.DirectorySeparatorChar);

        string normalizedImagePath = Path.GetFullPath(imagePath).TrimEnd(Path.DirectorySeparatorChar);

        if (!Path.Exists(imagePath))
        {
            logger.LogError("The image: {imagePath} does not exist", imagePath);
            return (false, NotFound("Image path not found."));
        }

        if (!normalizedImagePath.Contains(normalizedPhotoboxDirectory))
        {
            logger.LogError("The path: {imagePath} is not in the photobox directory", imagePath);
            return (false, StatusCode(StatusCodes.Status403Forbidden, "Access to the specified folder is forbidden."));
        }

        return (true, Ok("Image is being printed."));
    }


}
