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
}
