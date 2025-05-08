using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photobox.Lib;
using Photobox.Web.Database;

namespace Photobox.Web.Controllers;

public class EventController(AppDbContext dbContext) : Controller
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
}
