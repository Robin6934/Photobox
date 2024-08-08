using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photobox.Lib.Camera;
using Photobox.Lib.IPC;
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

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        await mainWindow.Start();
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
                services.AddTransient<MainWindow>();
                services.AddSingleton<ICamera, IPCNamedPipeClient>();
            })
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                // Register additional services if needed
            });
}

