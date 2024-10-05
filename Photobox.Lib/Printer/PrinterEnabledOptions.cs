using System.Text.Json.Serialization;

namespace Photobox.Lib.Printer;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrinterEnabledOptions
{
    /// <summary>
    /// Printing is enabled.
    /// </summary>
    True,

    /// <summary>
    /// Printing is disabled.
    /// </summary>
    False,

    /// <summary>
    /// When a printer is connected via USB, printing is enabled; otherwise, it is disabled.
    /// </summary>
    Automatic
}
