namespace Photobox.Web.Responses;

public record CheckPhotoboxResponse
{
    public bool Exists { get; set; }
    public string PhotoboxId { get; set; } = string.Empty;
}
