using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
using Photobox.LocalServer.RestApi.Api;
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
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IPhotoboxApi, PhotoboxApi>(s => new PhotoboxApi("https://localhost:7176"));
                services.AddSingleton<ICameraApi, CameraApi>(s => new CameraApi("https://localhost:7176"));
                services.AddSingleton<ISettingsApi, SettingsApi>(s => new SettingsApi("https://localhost:7176"));
                services.AddSingleton<MainWindow>();
                services.AddSingleton<LogWindow>();
                services.AddSingleton<ICamera, IPCNamedPipeClient>();
                services.AddSingleton<IImageViewer, ImageViewer.ImageViewer>();
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
            })
            .ConfigureAppConfiguration(config => config.AddEnvironmentVariables());
}

