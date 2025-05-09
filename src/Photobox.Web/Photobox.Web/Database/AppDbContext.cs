using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;
using Photobox.Web.Services;
using Image = Photobox.Web.Models.Image;

namespace Photobox.Web.Database;

public class AppDbContext(DbContextOptions<AppDbContext> context)
    : IdentityDbContext<ApplicationUser>(context)
{
    public virtual DbSet<Image> Images { get; init; }

    public virtual DbSet<PhotoBox> PhotoBoxes { get; init; }

    public virtual DbSet<Event> Events { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<ApplicationUser>()
            .HasMany(u => u.PhotoBoxes)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<ApplicationUser>()
            .HasMany(u => u.Events)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<Event>()
            .HasOne(e => e.PhotoBox)
            .WithOne(e => e.Event)
            .HasForeignKey<PhotoBox>(e => e.EventId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Entity<Event>()
            .HasMany(e => e.Images)
            .WithOne(i => i.Event)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
