using Photobox.Web.DbContext;
using Photobox.Web.Image;
using Photobox.Web.StorageProvider;

namespace Photobox.Web.Photobox;

public class PhotoBoxService(AppDbContext dbContext, ILogger<PhotoBoxService> logger)
{
    public async Task CreatePhotoBoxAsync()
    {

    }
}
