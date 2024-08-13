namespace Photobox.LocalServer.Models;

public record TakePictureResultModel
{
    public required string ImagePath { get; set; } = default!;
}
