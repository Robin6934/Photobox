using Microsoft.AspNetCore.Identity;

namespace Photobox.Web.Models;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public override Guid Id { get; set; }

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }

    public ICollection<PhotoBox> PhotoBoxes { get; } = [];

    public ICollection<Event> Events { get; } = [];
}
