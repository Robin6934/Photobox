namespace Photobox.Web.Dtos.Responses;

public record ImageUploadResponse
{
    public required string FileName { get; set; }
}
