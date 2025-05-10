using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Models;

[Index(nameof(ImageName))]
public class Image
{
    /// <summary>
    /// Primary key of the Image.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    /// <summary>
    /// The Name of the image, which got uploaded.
    /// </summary>
    [MaxLength(64)]
    public required string ImageName { get; set; }

    /// <summary>
    /// A Unique image name, used to store the image in the bucket storage.
    ///
    /// </summary>
    [MaxLength(45)]
    public required string UniqueImageName { get; set; }

    /// <summary>
    ///
    /// </summary>
    [MaxLength(45)]
    public required string DownscaledImageName { get; set; }

    public required DateTime TakenAt { get; set; }

    public Guid EventId { get; set; }

    [Required]
    public required Event Event { get; set; }
}
