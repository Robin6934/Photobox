using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.UI.CountDown;
using Photobox.UI.ImageViewer;
using Photobox.UI.Lib.Camera;
using Photobox.UI.Lib.ConfigModels;
using Photobox.UI.Lib.ImageHandler;
using Photobox.UI.Lib.ImageManager;
using Photobox.UI.Lib.ImageUploadService;
using Photobox.UI.Lib.Printer;
using Photobox.UI.Windows;
using Photobox.Web.RestApi.Api;
using Serilog;
using System.Windows;

namespace Photobox.UI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost host = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        host = CreateHostBuilder(e.Args).Build();

        await host.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await host.StopAsync();
        host.Dispose();
        base.OnExit(e);
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<MainWindow>();

        builder.Services.AddSingleton<IImageUploadService, ImageUploadService>();
        builder.Services.AddSingleton<CameraFactory>();
        builder.Services.AddSingleton(
            c => c.GetRequiredService<CameraFactory>()
                    .Create(c.GetRequiredService<IOptions<PhotoboxConfig>>().Value.Camera));
        builder.Services.AddSingleton<IImageViewer, ImageViewerLocal>();
        builder.Services.AddSingleton<IImageManager, ImageManager>();
        builder.Services.AddSingleton<IPrinter, Printer>();
        builder.Services.Configure<PhotoboxConfig>(
            builder.Configuration.GetSection(PhotoboxConfig.Photobox));
        builder.Services.AddSingleton<ICountDown, CountDownCircle>();
        builder.Services.AddSingleton<IImageHandler, ImageHandler>();
        builder.Services.AddSingleton<IImageApi, ImageApi>((IServiceProvider s) => new ImageApi("https://localhost"));

        builder.Logging.ClearProviders();

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Source", "UI")
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }
}

