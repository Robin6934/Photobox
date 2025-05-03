using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;

namespace Photobox.Web.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> context)
    : IdentityDbContext<ApplicationUser>(context)
{
    public virtual DbSet<Models.Image> ImageModels { get; init; }

    public virtual DbSet<PhotoBox> PhotoBoxModels { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<ApplicationUser>()
            .HasMany(u => u.PhotoBoxes)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
