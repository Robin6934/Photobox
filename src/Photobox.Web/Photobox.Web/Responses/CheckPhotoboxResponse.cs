namespace Photobox.Web.Responses;

public class CheckPhotoboxResponse
{
    public bool Exists { get; set; }
    public string PhotoboxId { get; set; } = string.Empty;
}
