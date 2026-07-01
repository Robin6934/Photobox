using System.Windows;
using Microsoft.Extensions.Hosting;

namespace Photobox.UI.Windows;

/// <summary>
/// Interaction logic for LogWindow.xaml
/// </summary>
public partial class LogWindow : Window, IHostedService
{
    public LogWindow()
    {
        InitializeComponent();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Show();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Close();
        return Task.CompletedTask;
    }
}
