using System.ComponentModel.DataAnnotations;

namespace Photobox.Web.Models;

public class ImageModel
{
    [Key]
    public long Id { get; set; }

    public required string ImageName { get; set; }

    public required string UniqeImageName { get; set; }

    public required string DownscaledImageName { get; set; }

    public required DateTime TakenAt { get; set; }
}
