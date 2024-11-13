using Autofac.Extensions.DependencyInjection;
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

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration(config =>
            {
                config.AddEnvironmentVariables();
                config.AddJsonFile("appsettings.json", true, true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<MainWindow>();
                services.AddSingleton<CameraFactory>();
                services.AddSingleton(
                    c => c.GetRequiredService<CameraFactory>()
                    .Create(c.GetRequiredService<IOptions<PhotoboxConfig>>().Value.Camera));
                services.AddSingleton<IImageViewer, ImageViewerLocal>();
                services.AddSingleton<IImageManager, ImageManager>();
                services.AddSingleton<IPrinter, Printer>();
                services.Configure<PhotoboxConfig>(
                    context.Configuration.GetSection(PhotoboxConfig.Photobox));
                services.AddSingleton<ICountDown, CountDownCircle>();
                services.AddSingleton<IImageHandler, ImageHandler>();
                services.AddSingleton<IImageApi, ImageApi>((IServiceProvider s) => new ImageApi("https://localhost:54829"));
            })
            .ConfigureLogging(logging => logging.ClearProviders())
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Source", "UI")
                    .WriteTo.Seq("http://localhost:5341");
            });
}

