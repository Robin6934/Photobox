using Microsoft.EntityFrameworkCore;
using Photobox.Web.Database;
using Photobox.Web.Models;

namespace Photobox.Web.Services;

public class EventService(AppDbContext dbContext)
{
    public Task<Event?> GetEventFromId(Guid eventId, CancellationToken cancellationToken = default)
    {
        return dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }

    public Task<Event?> GetEventFromPhotbox(
        PhotoBox photoBox,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Events.SingleOrDefaultAsync(
            e => e.UsedPhotoBoxId == photoBox.Id && e.IsActive,
            cancellationToken
        );
    }

    public Task<List<Event>> GetRunningEventsFromUserId(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Events.Where(x => x.IsActive == true).ToListAsync(cancellationToken);
    }

    public Task<Event?> GetEventFromEventCodeAsync(
        string eventCode,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Events.FirstOrDefaultAsync(
            e => e.EventCode == eventCode,
            cancellationToken
        );
    }

    public async Task<string> GenerateUniqueEventCodeAsync()
    {
        string code;
        bool exists;
        int maxRetries = 10;
        int attempt = 0;

        do
        {
            code = Generate6DigitCode();
            exists = await dbContext.Events.AnyAsync(e => e.EventCode == code);
            attempt++;

            if (attempt >= maxRetries)
                throw new InvalidOperationException(
                    "Failed to generate unique event code after multiple attempts."
                );
        } while (exists);

        return code;

        static string Generate6DigitCode()
        {
            var random = new Random();
            return random.Next(0, 1000000).ToString("D6"); // Always 6 digits, leading zeros allowed
        }
    }
}
