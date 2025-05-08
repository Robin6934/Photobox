namespace Photobox.Web.Requests;

public record RegisterPhotoboxRequest
{
    public required string PhotoBoxName { get; set; }
}
