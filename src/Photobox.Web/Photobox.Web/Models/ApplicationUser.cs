using Microsoft.AspNetCore.Identity;

namespace Photobox.Web.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<PhotoBox> PhotoBoxes { get; } = [];

    public ICollection<Event> Events { get; } = [];
}
