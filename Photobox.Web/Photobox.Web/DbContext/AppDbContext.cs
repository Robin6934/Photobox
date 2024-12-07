using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Photobox.Web.Image;
using Photobox.Web.Photobox;


namespace Photobox.Web.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> context) : IdentityDbContext<ApplicationUser>(context)
{
    public virtual DbSet<ImageModel> ImageModels { get; init; }

    public virtual DbSet<PhotoBoxModel> PhotoBoxModels { get; init; }
}
