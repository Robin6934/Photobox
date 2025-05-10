using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Models;

[Index(nameof(HardwareId))]
public class PhotoBox
{
    /// <summary>
    /// Primary key of the PhotoBox.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    /// <summary>
    /// Display name of the Photobox, shown in the UI. Max length: 256.
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
    public Guid ApplicationUserId { get; set; }

    /// <summary>
    /// Navigation property to the owning application user.
    /// </summary>
    public ApplicationUser ApplicationUser { get; set; }
}
