using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Photobox.Web.Models;

namespace Photobox.Web.Photobox;

public class PhotoBoxModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(52)]
    public required string PhotoboxId { get; set; }

    [MaxLength(50)]
    public string ApplicationUserId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }
}
