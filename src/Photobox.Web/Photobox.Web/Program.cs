using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using NSwag;
using NSwag.Generation.Processors.Security;
using Photobox.Web;
using Photobox.Web.Aws;
using Photobox.Web.Components;
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
    doc.AddSecurity(
        "bearer",
        new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Bearer token authorization header",
        }
    );

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

if (builder.Environment.IsStaging())
{
    builder.Configuration.AddEnvironmentVariables();
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddIdentityCookies();

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
    options.UseNpgsql(connectionString);
});

builder.Services.AddApplication();

builder.Services.ConfigureAws(builder.Configuration);

builder.Services.ConfigureHealthChecks(builder.Configuration);

var app = builder.Build();

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
            string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return await context.Users.FindAsync(userId);
        }
    )
    .RequireAuthorization();

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

app.UseHttpsRedirection();

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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Photobox.Web.Client._Imports).Assembly);

app.MapIdentityApi<ApplicationUser>();

app.Run();
