using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.UI.Lib.ConfigModels;

namespace Photobox.UI.Lib.PowerStatusWatcher;

[SupportedOSPlatform("windows")]
public partial class PowerStatusWatcher(
    IHostApplicationLifetime applicationLifetime,
    ILogger<PowerStatusWatcher> logger,
    IOptions<PhotoboxConfig> photoboxOptions
) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));

    private bool _previousStatus;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!photoboxOptions.Value.AutoOff)
        {
            logger.LogInformation("PowerStatusWatcher is not used.");
            return;
        }

        if (!GetSystemPowerStatus(out var initialStatus))
        {
            logger.LogWarning("Failed to retrieve initial power status.");
            return;
        }

        _previousStatus = initialStatus.ACLineStatus == 1;

        logger.LogInformation("PowerStatusWatcher started");

        while (
            await _timer.WaitForNextTickAsync(stoppingToken)
            && !stoppingToken.IsCancellationRequested
        )
        {
            GetSystemPowerStatus(out var systemPowerStatus);

            bool currentStatus = systemPowerStatus.ACLineStatus == 1;

            if (_previousStatus && !currentStatus)
            {
                logger.LogInformation(
                    "Powerstatus changed from plugged in to unplugged, shutting down computer..."
                );
                try
                {
                    applicationLifetime.StopApplication();
                    Process.Start("shutdown", "/s /t 0");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initiate shutdown.");
                }
            }

            _previousStatus = currentStatus;
        }

        logger.LogInformation("PowerStatusWatcher stopped");
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemPowerStatus
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetSystemPowerStatus", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetSystemPowerStatus(out SystemPowerStatus sps);
}
