using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Image;
using Photobox.Web.Models;
using Photobox.Web.Photobox;

namespace Photobox.Web.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> context)
    : IdentityDbContext<ApplicationUser>(context)
{
    public virtual DbSet<ImageModel> ImageModels { get; init; }

    public virtual DbSet<PhotoBoxModel> PhotoBoxModels { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<ApplicationUser>()
            .HasMany(u => u.PhotoBoxes)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PhotoBoxModel>().HasIndex(e => e.GalleryId).IsUnique();
    }
}
