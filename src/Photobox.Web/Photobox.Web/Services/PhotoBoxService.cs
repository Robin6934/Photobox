using Photobox.Web.DbContext;

namespace Photobox.Web.Services;

public class PhotoBoxService(AppDbContext dbContext, ILogger<PhotoBoxService> logger)
{
    public async Task CreatePhotoBoxAsync() { }
}
