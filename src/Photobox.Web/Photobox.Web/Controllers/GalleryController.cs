using Microsoft.AspNetCore.Mvc;
using Photobox.Web.Dtos.Responses;
using Photobox.Web.Dtos.Shared;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GalleryController(EventService eventService, ImageService imageService) : Controller
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult<ImageDownloadListResponse>> GetImagesFromGalleryCode(string code)
    {
        var @event = await eventService.GetEventFromEventCodeAsync(code);

        if (@event is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Event not found",
                    Detail = "No event is associated with the submitted gallery code.",
                }
            );
        }

        var imageNames = await imageService.GetImageNamesFromEventAsync(@event);

        var imageDtos = await Task.WhenAll(
            imageNames.Select(async imageName =>
            {
                string downscaledUrl = await imageService.GetPreviewImagePreSignedUrl(imageName);

                return new ImageDownloadDto
                {
                    OriginalImageUrl = $"/api/Image/GetImage/{imageName}",
                    DownscaledImageUrl = downscaledUrl,
                };
            })
        );

        return Ok(new ImageDownloadListResponse { Images = imageDtos.ToList() });
    }
}
