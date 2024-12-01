using Microsoft.EntityFrameworkCore;
using Photobox.Web.Models;

namespace Photobox.Web.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> context) : Microsoft.EntityFrameworkCore.DbContext(context)
{
    public virtual DbSet<UserModel> UserModels { get; init; }

    public virtual DbSet<ImageModel> ImageModels { get; init; }
}
