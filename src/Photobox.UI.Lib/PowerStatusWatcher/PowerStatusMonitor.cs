using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Photobox.UI.Lib.PowerStatusWatcher;

[SupportedOSPlatform("windows")]
public class PowerStatusMonitor(
    ILogger<PowerStatusMonitor> logger,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    private bool _pluggedIn;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("PowerStatusMonitor starting...");

        _pluggedIn = IsPluggedIn();
        SystemEvents.PowerModeChanged += OnPowerModeChanged;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        logger.LogInformation("PowerStatusMonitor stopped.");
        return Task.CompletedTask;
    }

    private void OnPowerModeChanged(object? sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode != PowerModes.StatusChange)
            return;

        bool currentlyPluggedIn = IsPluggedIn();

        if (_pluggedIn && !currentlyPluggedIn)
        {
            logger.LogWarning("Power source unplugged. Initiating shutdown...");

            try
            {
                appLifetime.StopApplication();
                Process.Start("shutdown", "/s /t 0");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to shut down the system.");
            }
        }

        _pluggedIn = currentlyPluggedIn;
    }

    private static bool IsPluggedIn()
    {
        GetSystemPowerStatus(out SYSTEM_POWER_STATUS status);
        return status.ACLineStatus == 1;
    }

    [DllImport("kernel32.dll")]
    private static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

    private struct SYSTEM_POWER_STATUS
    {
        public byte ACLineStatus; // 0 = offline, 1 = online
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }
}
