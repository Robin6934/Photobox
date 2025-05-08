using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Photobox.Web.Models;

public class Event
{
    /// <summary>
    /// Primary key of the PhotoBox (generated as GUID v7).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Name of the current Event
    /// </summary>
    public required string Name { get; set; }

    public PhotoBox? PhotoBox { get; set; }

    public ICollection<Image> Images { get; } = [];

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
}
