namespace Photobox.Lib.Models;

public record TakePictureResultModel
{
    public required string ImagePath { get; set; } = default!;
}
