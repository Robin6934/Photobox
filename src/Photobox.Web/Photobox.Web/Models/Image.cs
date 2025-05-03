using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Photobox.Web.Models;

[Index(nameof(ImageName))]
public class Image
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
