using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Database;
using Photobox.Web.Models;

namespace Photobox.Web.Services;

public class PhotoBoxService(
    AppDbContext dbContext,
    ILogger<PhotoBoxService> logger,
    IValidator<PhotoBox> validator
)
{
    public async Task<bool> CreateAsync(
        PhotoBox photoBox,
        CancellationToken cancellationToken = default
    )
    {
        await validator.ValidateAsync(photoBox, cancellationToken);
        await dbContext.AddAsync(photoBox, cancellationToken);

        var result = await dbContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<PhotoBox?> GetFromHardwareIdAsync(
        string hardwareId,
        CancellationToken cancellationToken = default
    )
    {
        var photobox = await dbContext.PhotoBoxes.FirstOrDefaultAsync(
            p => p.HardwareId == hardwareId,
            cancellationToken
        );

        return photobox;
    }

    public async Task<PhotoBox?> GetFromIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var photobox = await dbContext.PhotoBoxes.FirstOrDefaultAsync(
            p => p.Id == id,
            cancellationToken
        );

        return photobox;
    }

    public Task<List<PhotoBox>> GetPhotoboxesFromUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext
            .PhotoBoxes.Where(p => p.ApplicationUserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task DeletePhotoboxByIdAsync(
        Guid photoboxId,
        CancellationToken cancellationToken = default
    )
    {
        var photobox = await dbContext.PhotoBoxes.FirstOrDefaultAsync(
            p => p.Id == photoboxId,
            cancellationToken
        );

        ArgumentNullException.ThrowIfNull(photobox);

        dbContext.PhotoBoxes.Remove(photobox);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
