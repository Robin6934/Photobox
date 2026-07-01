namespace Photobox.Web.Responses;

public record ImageUploadResponse
{
    public required string FileName { get; set; }
}
