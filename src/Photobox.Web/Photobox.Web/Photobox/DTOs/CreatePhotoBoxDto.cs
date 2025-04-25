namespace Photobox.Web.Photobox.DTOs;

public class CreatePhotoBoxDto
{
    public required string PhotoBoxId { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }

    public required string PhotoBoxName { get; set; }
}
