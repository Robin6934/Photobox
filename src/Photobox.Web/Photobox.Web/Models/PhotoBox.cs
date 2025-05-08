using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Models;

[Index(nameof(HardwareId))]
public class PhotoBox
{
    /// <summary>
    /// Primary key of the PhotoBox (generated as GUID v7).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the photobox, shown in the UI. Max length: 50.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    /// <summary>
    /// Stable identifier assigned to a specific device. Used for registration.
    /// </summary>
    [Required]
    [MaxLength(52)]
    public required string HardwareId { get; set; }

    /// <summary>
    /// Foreign key to the owning application user.
    /// </summary>
    [MaxLength(50)]
    public string ApplicationUserId { get; set; }

    /// <summary>
    /// Navigation property to the owning application user.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }

    /// <summary>
    /// Foreign key to the owning event.
    /// </summary>
    public Guid? EventId { get; set; }

    /// <summary>
    /// Navigation property to the owning event.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
