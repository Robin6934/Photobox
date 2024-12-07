using Photobox.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Photobox.Web.Photobox;

public class PhotoBoxModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string UserId { get; set; } = default!;

    public ApplicationUser User { get; set; } = default!;
}
