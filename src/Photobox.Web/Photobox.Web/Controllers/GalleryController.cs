using Microsoft.AspNetCore.Mvc;
using Photobox.Web.Dtos.Responses;
using Photobox.Web.Dtos.Shared;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GalleryController(GalleryService galleryService) : Controller
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult<ImageDownloadListResponse>> GetImagesFromGalleryCode(string code)
    {
        var images = await galleryService.GetImageUrlsByEventCodeAsync(code);

        return Ok(
            new ImageDownloadListResponse
            {
                Images = images
                    .Select(x => new ImageDownloadDto
                    {
                        OriginalImageUrl = x.DownloadUrl,
                        DownscaledImageUrl = x.PreviewPresignedUrl,
                    })
                    .ToList(),
            }
        );
    }
}
