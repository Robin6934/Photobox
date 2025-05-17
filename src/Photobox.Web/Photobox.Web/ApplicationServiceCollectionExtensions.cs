using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Photobox.Web.Components.Account;
using Photobox.Web.Services;

namespace Photobox.Web;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ImageService>();

        services.AddScoped<PhotoBoxService>();

        services.AddScoped<EventService>();

        services.AddSingleton<IStorageService, AwsStorageService>();

        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

        services.AddScoped<IdentityRedirectManager>();

        services.AddScoped<IdentityUserAccessor>();

        services.AddScoped<
            AuthenticationStateProvider,
            IdentityRevalidatingAuthenticationStateProvider
        >();

        return services;
    }
}
