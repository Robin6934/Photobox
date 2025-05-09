using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Photobox.Lib;
using Photobox.Web.Database;
using Photobox.Web.Models;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

public class EventController(AppDbContext dbContext, ImageService imageService) : Controller
{
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

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok();
    }

    [NonAction]
    public async Task<int> SaveChangesAsync(Event deletedEvent, CancellationToken cancellationToken)
    {
        var imageKeysToDelete = await dbContext
            .Images.Where(i => deletedEvent.Id == i.EventId)
            .ToListAsync(cancellationToken);

        await imageService.DeleteImagesAsync(imageKeysToDelete);

        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
