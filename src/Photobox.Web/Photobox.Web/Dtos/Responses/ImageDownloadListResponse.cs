using Photobox.Web.Dtos.Shared;

namespace Photobox.Web.Dtos.Responses;

public class ImageDownloadListResponse
{
    public required List<ImageDownloadDto> Images { get; set; }
}
