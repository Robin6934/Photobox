using Image = Photobox.Web.Models.Image;

namespace Photobox.Web.Services;

public class GalleryService(EventService eventService, ImageService imageService)
{
    public async Task<List<Image>> GetImagesByEventCodeAsync(string eventCode)
    {
        var @event = await eventService.GetEventFromEventCodeAsync(eventCode);

        if (@event is null)
            throw new ArgumentException("Invalid event code");

        return await imageService.GetImageFromEventAsync(@event);
    }
}
