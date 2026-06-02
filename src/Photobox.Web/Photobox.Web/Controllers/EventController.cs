using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Photobox.Lib;
using Photobox.Web.Database;
using Photobox.Web.Dtos.Responses;
using Photobox.Web.Mapping;
using Photobox.Web.Models;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class EventController(
    AppDbContext dbContext,
    ImageService imageService,
    PhotoBoxService photoBoxService,
    EventService eventService
) : Controller
{
    [ProducesResponseType<GalleryCodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetGalleryCode(
        [FromHeader(Name = PhotoboxHeaders.HardwareId)] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        var photobox = await photoBoxService.GetFromHardwareIdAsync(photoBoxId, cancellationToken);

        if (photobox is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Photobox not found",
                    Detail = "No photobox found with the submitted hardware ID.",
                }
            );
        }

        var @event = await eventService.GetEventFromPhotbox(photobox, cancellationToken);

        if (@event is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Event not found",
                    Detail = "No event is associated with the submitted photobox ID.",
                }
            );
        }

        return Ok(@event.MapToGalleryCodeResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok();
    }
}
