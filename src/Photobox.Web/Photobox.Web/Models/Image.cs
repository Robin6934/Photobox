using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Models;

[Index(nameof(ImageName))]
public class Image
{
    /// <summary>
    /// Primary key of the Image (generated as GUID v7).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [MaxLength(64)]
    public required string ImageName { get; set; }

    //Length of a uuid plus the file extension
    [MaxLength(45)]
    public required string UniqueImageName { get; set; }

    //Length of a uuid plus the file extension
    [MaxLength(45)]
    public required string DownscaledImageName { get; set; }

    public required DateTime TakenAt { get; set; }

    public Guid EventId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(EventId))]
    [Required]
    public Event? Event { get; set; }
}
