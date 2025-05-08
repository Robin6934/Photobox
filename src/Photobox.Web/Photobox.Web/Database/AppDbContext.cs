using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;
using Photobox.Web.Services;
using Image = Photobox.Web.Models.Image;

namespace Photobox.Web.Database;

public class AppDbContext(DbContextOptions<AppDbContext> context, ImageService imageService)
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var deletedEvents = ChangeTracker
            .Entries<Event>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        if (deletedEvents.Count != 0)
        {
            var imageKeysToDelete = await Images
                .Where(i => deletedEvents.Select(ev => ev.Id).Contains(i.EventId))
                .ToListAsync(cancellationToken);

            await imageService.DeleteImagesAsync(imageKeysToDelete);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
