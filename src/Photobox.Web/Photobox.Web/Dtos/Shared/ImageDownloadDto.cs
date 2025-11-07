namespace Photobox.Web.Dtos.Shared;

public class ImageDownloadDto
{
    public required string OriginalImageUrl { get; set; }
    public required string DownscaledImageUrl { get; set; }
}
