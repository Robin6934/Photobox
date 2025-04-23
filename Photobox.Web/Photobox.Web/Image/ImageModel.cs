using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Image;

[Index(nameof(ImageName))]
public class ImageModel
{
    [Key]
    public long Id { get; set; }

    [MaxLength(64)]
    public required string ImageName { get; set; }

    //Length of a uuid plus the file extension
    [MaxLength(45)]
    public required string UniqueImageName { get; set; }

    //Length of a uuid plus the file extension
    [MaxLength(45)]
    public required string DownscaledImageName { get; set; }

    public required DateTime TakenAt { get; set; }
}
