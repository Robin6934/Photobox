using Microsoft.AspNetCore.Identity;
using Photobox.Web.Photobox;

namespace Photobox.Web.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<PhotoBoxModel> PhotoBoxes { get; } = [];
}
