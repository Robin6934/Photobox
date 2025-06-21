using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Photobox.Lib;
using Photobox.Web.Database;
using Photobox.Web.Models;
using Photobox.Web.Responses;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class EventController(AppDbContext dbContext, ImageService imageService) : Controller
{
    [ProducesResponseType<GalleryCodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetGalleryCode(
        [FromHeader(Name = PhotoboxHeaders.HardwareId)] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        var photobox = await dbContext.PhotoBoxes.FirstOrDefaultAsync(
            e => e.HardwareId == photoBoxId,
            cancellationToken
        );

        if (photobox is null)
        {
            return NotFound();
        }

        var @event = await dbContext.Events.FirstOrDefaultAsync(
            x => x.UsedPhotoBoxId == photobox.Id && x.IsActive,
            cancellationToken
        );

        return Ok(new GalleryCodeResponse { Code = @event.EventCode });
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok();
    }
}
