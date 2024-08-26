using Microsoft.Extensions.Options;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.LocalServer.ConfigModels;
using Serilog;

namespace Photobox.LocalServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        builder.Configuration.AddJsonFile("appsettings.json", true, true);

        builder.Services.Configure<PhotoboxConfig>(
            builder.Configuration.GetSection(PhotoboxConfig.Photobox));

        builder.Services.AddSingleton<CameraFactory>();
        builder.Services.AddSingleton(s => s.GetRequiredService<CameraFactory>().Create());
        builder.Services.AddSingleton<IIPCServer, IPCNamedPipeServer>();
        builder.Services.AddSingleton<IPrinter, Printer>();
        builder.Services.AddSingleton<IImageManager, ImageManager>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseSerilogRequestLogging(); // Log HTTP requests

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
