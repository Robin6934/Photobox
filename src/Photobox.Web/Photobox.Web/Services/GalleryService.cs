using Photobox.Web.Dtos.Shared;
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

    public async Task<List<string>> GetImageNamesByEventCodeAsync(string eventCode)
    {
        var @event = await eventService.GetEventFromEventCodeAsync(eventCode);

        if (@event is null)
            throw new ArgumentException("Invalid event code");

        return await imageService.GetImageNamesFromEventAsync(@event);
    }

    public async Task<List<ImageUrls>> GetImageUrlsByEventCodeAsync(string eventCode)
    {
        List<ImageUrls> urls = [];

        var @event = await eventService.GetEventFromEventCodeAsync(eventCode);

        if (@event is null)
            throw new ArgumentException("Invalid event code");

        var imageNames = await imageService.GetImageNamesFromEventAsync(@event);

        foreach (var imageName in imageNames)
        {
            urls.Add(
                new ImageUrls
                {
                    DownloadUrl = $"/api/Image/GetImage/{imageName}",
                    PreviewPresignedUrl = await imageService.GetPreviewImagePreSignedUrl(imageName),
                }
            );
        }

        return urls;
    }
}
