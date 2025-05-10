using Microsoft.EntityFrameworkCore;
using Photobox.Web.Database;
using Photobox.Web.Models;

namespace Photobox.Web.Services;

public class EventService(AppDbContext dbContext)
{
    public string GetGalleryCode(string hardwareId)
    {
        return "";
    }

    public Task<Event?> GetEventFromId(Guid eventId, CancellationToken cancellationToken = default)
    {
        return dbContext.Events.Where(e => e.Id == eventId).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Event?> GetEventFromPhotbox(
        PhotoBox photoBox,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Events.SingleOrDefaultAsync(
            e => e.UsedPhotoBoxId == photoBox.Id && e.IsActive,
            cancellationToken
        );
    }
}
