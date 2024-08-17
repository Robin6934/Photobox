using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Serilog;
using Serilog.Events;

namespace Photobox.LocalServer;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddOpenApiDocument();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Host.UseSerilog();

        builder.Services.AddSingleton<ICamera, WebCam>();
        builder.Services.AddSingleton<IIPCServer, IPCNamedPipeServer>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseOpenApi();
            app.UseSwaggerUI();
        }
        app.UseSerilogRequestLogging(); // Log HTTP requests

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
