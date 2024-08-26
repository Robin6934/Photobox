using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Photobox.LocalServer.ConfigModels;
using Photobox.LocalServer.Enums;
using Photobox.LocalServer.Models;
using System.Drawing.Printing;
using System.Management;

namespace Photobox.LocalServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SettingsController(ILogger<SettingsController> logger, IOptionsMonitor<PhotoboxConfig> options) : Controller
{
    private readonly PhotoboxConfig photoboxConfig = options.CurrentValue;

    private readonly ILogger<SettingsController> logger = logger;

    /// <summary>
    /// Retrieves a list of all installed printers.
    /// </summary>
    /// <returns>A list of installed printers available on the system.</returns>
    /// <response code="200">Returns the list of printers.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListPrintersResultModel))]
    public IActionResult ListPrinters()
    {
        ListPrintersResultModel resultModel = new()
        {
            Printers = GetAllPrinters()
        };

        return Ok(resultModel);
    }

    private static List<string> GetAllPrinters()
    {
        List<string> printers = [];

        foreach (string item in PrinterSettings.InstalledPrinters)
        {
            printers.Add(item);
        }

        return printers;
    }

    /// <summary>
    /// Retrieves the current Photobox settings.
    /// </summary>
    /// <returns>The current configuration of the Photobox application.</returns>
    /// <response code="200">Returns the Photobox settings.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PhotoboxConfig))]

    public IActionResult GetPhotoboxSettings()
    {
        return Ok(photoboxConfig);
    }

    /// <summary>
    /// Sets the default printer for the Photobox application.
    /// </summary>
    /// <param name="printerName">The name of the printer to set as the default.</param>
    /// <returns>A status indicating if the printer was successfully set.</returns>
    /// <response code="200">The printer was successfully set.</response>
    /// <response code="404">The specified printer was not found.</response>
    [HttpGet("{printerName}")]
    public IActionResult SetPrinter(string printerName)
    {
        if (!GetAllPrinters().Contains(printerName))
        {
            return NotFound($"The printer '{printerName}' was not found.");
        }

        photoboxConfig.PrinterName = printerName;

        photoboxConfig.Save();

        return Ok();
    }


    /// <summary>
    /// Sets the printing enabled option in the Photobox application.
    /// </summary>
    /// <param name="printerEnabled">The printing enabled option (True, False, Automatic).</param>
    /// <returns>A status indicating if the setting was successfully applied.</returns>
    /// <response code="200">The setting was successfully applied.</response>
    [HttpGet("{printerEnabled}")]
    public IActionResult SetPrinterEnabled(PrinterEnabledOptions printerEnabled)
    {
        photoboxConfig.PrintingEnabled = printerEnabled;
        photoboxConfig.Save();
        return Ok();
    }

    /// <summary>
    /// Checks whether printing is enabled based on the current configuration.
    /// </summary>
    /// <returns>A status indicating whether printing is enabled.</returns>
    /// <response code="200">Returns whether printing is enabled.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PrintingEnabledResultModel))]
    public IActionResult PrintingEnabled()
    {
        PrintingEnabledResultModel result = new()
        {
            PrintingEnabled = photoboxConfig.PrintingEnabled switch
            {
                PrinterEnabledOptions.True => true,
                PrinterEnabledOptions.False => false,
                PrinterEnabledOptions.Automatic => CheckIfPrinterIsConnected(photoboxConfig.PrinterName), // Example method to check printer status
                _ => false // Default case, if necessary
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Checks if a printer is connected via USB.
    /// </summary>
    /// <returns>A boolean value indicating whether a printer is connected via usb.</returns>
    private bool CheckIfPrinterIsConnected(string printerToCheck)
    {
        // Replace single quotes in printer name to prevent query errors
        string sanitizedPrinterName = printerToCheck.Replace("'", "''");

        // Search for the printer by name
        using var searcher = new ManagementObjectSearcher($"SELECT Name, PrinterStatus, DetectedErrorState, WorkOffline FROM Win32_Printer WHERE Name LIKE '%{sanitizedPrinterName}%'");

        foreach (var printer in searcher.Get())
        {
            string printerName = printer["Name"]?.ToString();

            if (string.Equals(printerName, printerToCheck, StringComparison.OrdinalIgnoreCase))
            {
                // Check if the printer is available and ready to print
                int printerStatus = Convert.ToInt32(printer["PrinterStatus"]); // Status of the printer
                int detectedState = Convert.ToInt32(printer["DetectedErrorState"]); // Error state of the printer
                bool isPrinterOnline = !Convert.ToBoolean(printer["WorkOffline"]); // Checks if the printer is online

                // PrinterStatus 3 means it's idle (ready to print)
                // DetectedErrorState 0 means no error
                // WorkOffline should be false
                if (printerStatus == 3 && detectedState == 0 && isPrinterOnline)
                {
                    return true; // Printer is available and ready to print
                }
                else
                {
                    return false; // Printer is either not available or not ready
                }
            }
        }

        // If the printer was not found, consider it not connected
        return false;
    }


}
