using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.PhotoboxSettingsManager;
using Photobox.Lib.RestApi;
using Photobox.UI.CountDown;
using Photobox.UI.ImageViewer;
using Photobox.UI.Lib.Camera;
using Photobox.UI.Lib.ConfigModels;
using Photobox.UI.Lib.ImageHandler;
using Photobox.UI.Lib.ImageManager;
using Photobox.UI.Lib.ImageUploadService;
using Photobox.UI.Lib.PowerStatusWatcher;
using Photobox.UI.Lib.Printer;
using Photobox.UI.Windows;
using Serilog;
using System.IO.Abstractions;

namespace Photobox.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private IHost _host = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        _host = CreateHostBuilder(e.Args).Build();

        ImageClient.AccessTokenManager = _host.Services.GetService<IAccessTokenManager>();
        PhotoBoxClient.AccessTokenManager = _host.Services.GetService<IAccessTokenManager>();

        await _host.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        string url =
            builder.Configuration.GetSection(PhotoboxConfig.Photobox)["ServerUrl"]
            ?? "https://localhost";

        builder.Services.AddHostedService<MainWindow>();
        builder.Services.AddHostedService<PowerStatusWatcher>();

        builder.Services.AddSingleton<IImageUploadService, ImageUploadService>();
        builder.Services.AddSingleton<CameraFactory>();
        builder.Services.AddSingleton(s =>
            s.GetRequiredService<CameraFactory>()
                .Create(s.GetRequiredService<IOptions<PhotoboxConfig>>().Value.Camera)
        );
        builder.Services.AddSingleton<IImageViewer, ImageViewerLocal>();
        builder.Services.AddSingleton<IImageManager, ImageManager>();
        builder.Services.AddSingleton<IPrinter, Printer>();
        builder.Services.Configure<PhotoboxConfig>(
            builder.Configuration.GetSection(PhotoboxConfig.Photobox)
        );
        builder.Services.AddSingleton<ICountDown, CountDownCircle>();
        builder.Services.AddSingleton<IImageHandler, ImageHandler>();
        builder.Services.AddSingleton<IImageClient, ImageClient>(_ => new ImageClient(url));
        builder.Services.AddSingleton<IClient, Client>(s => new Client(url));
        builder.Services.AddSingleton<IPhotoBoxClient, PhotoBoxClient>(_ => new PhotoBoxClient(
            url
        ));
        builder.Services.AddSingleton<IAccessTokenManager, AccessTokenManager>();
        builder.Services.AddSingleton<IPhotoboxSettingsManager, PhotoboxSettingsManager>();
        builder.Services.AddSingleton<IFileSystem, FileSystem>();
        builder.Logging.ClearProviders();

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Source", "UI")
            .WriteTo.Seq("http://localhost:5341")
            .WriteTo.File("Photobox.log")
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }
}
