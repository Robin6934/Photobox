using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;

namespace Photobox.Web.DbContext;

public class MariaDbContext(DbContextOptions<MariaDbContext> context) : Microsoft.EntityFrameworkCore.DbContext(context)
{
    public virtual DbSet<UserModel> UserModels { get; set; }

    public virtual DbSet<ImageModel> ImageModels { get; set; }
}
