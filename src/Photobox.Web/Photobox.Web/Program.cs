using System.Collections;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using NSwag;
using NSwag.Generation.Processors.Security;
using Photobox.Web;
using Photobox.Web.Aws;
using Photobox.Web.Components;
using Photobox.Web.Components.Account;
using Photobox.Web.Database;
using Photobox.Web.HealthCheck;
using Photobox.Web.Models;
using Scalar.AspNetCore;
using Serilog;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApiDocument(doc =>
{
    doc.Title = "Photobox API";

    // Add a JWT Bearer security scheme
    doc.AddSecurity(
        "bearer",
        new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http, // Use Http type for Bearer tokens
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter 'Bearer {token}' to authenticate",
        }
    );

    // Apply the security scheme to all operations automatically
    doc.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddImageSharp();

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

if (builder.Environment.IsDevelopment() && builder.Environment.ApplicationName is { Length: > 0 })
{
    var assembly = Assembly.Load(builder.Environment.ApplicationName);

    builder.Configuration.AddUserSecrets(assembly, optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder
    .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(
        IdentityConstants.ApplicationScheme,
        options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.LoginPath = "/account/login"; // for Razor Pages
            options.Events.OnRedirectToLogin = context =>
            {
                if (
                    context.Request.Path.StartsWithSegments("/api")
                    || context.Request.Path.StartsWithSegments("/users")
                )
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                if (
                    context.Request.Path.StartsWithSegments("/api")
                    || context.Request.Path.StartsWithSegments("/users")
                )
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        }
    )
    .AddBearerToken(IdentityConstants.BearerScheme);

// builder
//     .Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = IdentityConstants.ApplicationScheme;
//     })
//     .AddBearerToken(IdentityConstants.BearerScheme)
//     .AddCookie();
//
//
//
// builder.Services.AddAuthorization(options =>
// {
//     var defaultAuthorizationPolicy = new AuthorizationPolicyBuilder(
//         IdentityConstants.ApplicationScheme,
//         IdentityConstants.BearerScheme);
//
//     options.DefaultPolicy = defaultAuthorizationPolicy.RequireAuthenticatedUser().Build();
//
//     options.AddPolicy();
// });

builder
    .Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// Add services to the container.
builder
    .Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddMemoryCache();

builder.Host.UseSerilog(
    (context, services, configuration) =>
    {
        configuration
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Source", "Server")
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:80");
    }
);

builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PhotoboxConnectionString");
    Console.WriteLine("Connecting to db: " + connectionString);
    options.UseNpgsql(connectionString);
});

builder.Services.AddApplication();

builder.Services.ConfigureAws(builder.Configuration);

builder.Services.ConfigureHealthChecks(builder.Configuration);

var app = builder.Build();

var enviroment = Environment.GetEnvironmentVariables();

Console.WriteLine("Enviroment");
foreach (DictionaryEntry variable in enviroment)
{
    Console.WriteLine($"{variable.Key} = {variable.Value}");
}

Console.WriteLine("Config");
foreach (var config in builder.Configuration.AsEnumerable())
{
    Console.WriteLine($"{config.Key} = {config.Value}");
}

app.UseSerilogRequestLogging();

//HealthCheck Middleware
app.MapHealthChecks(
    "/api/health",
    new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    }
);

app.MapGet(
        "users/me",
        async (ClaimsPrincipal claims, AppDbContext context) =>
        {
            Guid userId = Guid.Parse(
                claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );

            return await context.Users.FindAsync(userId);
        }
    )
    .RequireAuthorization(policy =>
        policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme).RequireAuthenticatedUser()
    );

// Bearer tokens are client-side cleared; no server-side revocation needed
app.MapPost("/api/logout", () => Results.Ok());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseOpenApi(options =>
    {
        options.Path = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseImageSharp();

app.MapControllers();

app.UseAntiforgery();

// needed because of nswag generator, when the manifestpath is set to null,
// usually it takes the application name and appends .staticwebassets.endpoints.json
// but here it just takes null, so we need to provide the path manually
var manifestPath = Path.Combine(
    AppContext.BaseDirectory,
    "Photobox.Web.staticwebassets.endpoints.json"
);

app.MapStaticAssets(manifestPath);

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode();

app.MapGroup("api").MapIdentityApi<ApplicationUser>();

app.MapAdditionalIdentityEndpoints();

app.Run();
