using DeviceId;

namespace Photobox.UI.Lib.Helper;

public static class PhotoboxHelper
{
    static string PhotoboxId => 
        new DeviceIdBuilder()
        .AddMacAddress()
        .OnWindows(windows => windows
            .AddProcessorId()
            .AddMotherboardSerialNumber())
        .ToString();
}