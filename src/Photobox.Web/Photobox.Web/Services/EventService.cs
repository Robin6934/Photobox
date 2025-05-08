using Microsoft.EntityFrameworkCore;
using Photobox.Web.Database;

namespace Photobox.Web.Services;

public class EventService(AppDbContext dbContext)
{
    public string GetGalleryCode(string hardwareId)
    {
        var photobox = dbContext
            .PhotoBoxes.Include(photoBox => photoBox.Event)
            .First(p => p.HardwareId == hardwareId);

        return photobox.Event.Name;
    }
}
