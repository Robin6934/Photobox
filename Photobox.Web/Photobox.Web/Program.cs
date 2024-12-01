using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Aws;
using Photobox.Web.Components;
using Photobox.Web.DbContext;
using Photobox.Web.HealthCheck;
using Photobox.Web.Image;
using Photobox.Web.StorageProvider;
using Serilog;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument();

builder.Services.AddImageSharp();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Source", "Server")
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:80");
});

builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PhotoboxConnectionString");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ImageService>();

builder.Services.AddSingleton<IStorageProvider, AwsStorageProvider>();

builder.Services.ConfigureAws(builder.Configuration);

builder.Services.ConfigureHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

//HealthCheck Middleware
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseOpenApi();
    app.UseSwaggerUI();
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

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Photobox.Web.Client._Imports).Assembly);

app.Run();

