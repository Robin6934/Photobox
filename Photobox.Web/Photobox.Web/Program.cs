using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.Components;
using Photobox.Web.DbContext;
using Photobox.Web.Image;
using Photobox.Web.StorageProvider;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContextPool<MariaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PhotoboxConnectionString");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<ImageService>();

builder.Services.AddSingleton<IStorageProvider, AwsStorageProvider>();

ConfigureAws(builder.Configuration, builder.Services);



var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Photobox.Web.Client._Imports).Assembly);

app.Run();


void ConfigureAws(IConfiguration configuration, IServiceCollection services)
{
    var serviceUrl = configuration["AWS:ServiceURL"];
    var accessKey = configuration["AWS:AccessKey"];
    var secretKey = configuration["AWS:SecretKey"];

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    var s3Config = new AmazonS3Config
    {
        ServiceURL = serviceUrl,
        ForcePathStyle = true // Ensure compatibility with Cloudflare R2
    };

    // Directly register IAmazonS3 with specified config
    services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, s3Config));
}

