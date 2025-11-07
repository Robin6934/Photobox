namespace Photobox.Web.Dtos.Requests;

public record RegisterPhotoboxRequest
{
    public required string PhotoBoxName { get; set; }
}
