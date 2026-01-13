using Photobox.Web.Dtos.Responses;
using Photobox.Web.Models;

namespace Photobox.Web.Mapping;

public static class EventMapping
{
    public static GalleryCodeResponse MapToGalleryCodeResponse(this Event @event)
    {
        return new GalleryCodeResponse { Code = @event.EventCode };
    }
}
