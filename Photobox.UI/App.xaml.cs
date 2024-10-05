using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Camera;
using Photobox.Lib.ConfigModels;
using Photobox.Lib.PhotoManager;
using Photobox.Lib.Printer;
using Photobox.UI.ImageViewer;
using Photobox.UI.Windows;
using Serilog;
using System.Windows;

namespace Photobox.UI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost host = default!;

    private MainWindow mainWindow = default!;

    private LogWindow logWindow = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        host = CreateHostBuilder(e.Args).Build();
        await host.StartAsync();

        mainWindow = host.Services.GetRequiredService<MainWindow>();
        logWindow = host.Services.GetRequiredService<LogWindow>();
        await mainWindow.Start();
        logWindow.Show();
        mainWindow.Show();
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
                config.AddJsonFile(".\\appsettings.json", true, true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<LogWindow>();
                services.AddSingleton<CameraFactory>();
                services.AddSingleton(c => c.GetRequiredService<CameraFactory>().Create());
                services.AddSingleton<IImageViewer, ImageViewerLocal>();
                services.AddSingleton<IImageManager, ImageManager>();
                services.AddSingleton<IPrinter, Printer>();
                services.Configure<PhotoboxConfig>(
                    context.Configuration.GetSection(PhotoboxConfig.Photobox));
            })
            .ConfigureLogging(logging => logging.ClearProviders())
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Source", "UI")
                    .WriteTo.RichTextBox(services.GetRequiredService<LogWindow>().RichTextBoxLog)
                    .WriteTo.Seq("http://localhost:5341");
            });
}

