using FluentValidation;
using Photobox.Web.Services;

namespace Photobox.Web;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ImageService>();

        services.AddScoped<PhotoBoxService>();

        services.AddSingleton<IStorageService, AwsStorageService>();

        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

        return services;
    }
}
