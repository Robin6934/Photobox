namespace Photobox.Web.Photobox.DTOs;

public record LoginPhotoboxDto
{
    public required string UserName { get; set; }

    public required string Password { get; set; }

    public required Guid PhotoBoxId { get; set; }
}
