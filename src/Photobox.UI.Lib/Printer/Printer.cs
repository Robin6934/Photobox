﻿using System.Buffers;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photobox.Lib.Extensions;
using Photobox.UI.Lib.ConfigModels;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.UI.Lib.Printer;

public class Printer(ILogger<Printer> logger, IOptions<PhotoboxConfig> options) : IPrinter
{
    public bool Enabled => PrintingEnabled();

    public List<string> ListPrinters()
    {
        List<string> printers = [];

        foreach (string item in PrinterSettings.InstalledPrinters)
        {
            printers.Add(item);
        }

        logger.LogInformation("Found the Printers: {printerList}", string.Join(";\n", printers));

        return printers;
    }

    public bool SetPrinter(string printerName)
    {
        if (!ListPrinters().Contains(printerName))
        {
            logger.LogError("The printer {printerName} was not found", printerName);
            return false;
        }

        options.Value.PrinterName = printerName;

        options.Value.Save();

        return true;
    }

    public void SetPrinterEnabled(PrinterEnabledOptions printerEnabled)
    {
        options.Value.PrintingEnabled = printerEnabled;
        options.Value.Save();
    }

    public async Task PrintAsync(SixLabors.ImageSharp.Image<Rgb24> image)
    {
        string printerName = options.Value.PrinterName;
        var tcs = new TaskCompletionSource<bool>();

        logger.LogInformation("Printing of image on printer: {printerName} started.", printerName);

        Bitmap bitmap = new(await image.ToJpegStreamAsync());

        PrinterSettings printerSettings = new() { PrinterName = printerName };

        using PrintDocument pd = new() { PrinterSettings = printerSettings };

        pd.PrintPage += (sender, e) =>
        {
            float width = e.PageSettings.PrintableArea.Width;
            float height = e.PageSettings.PrintableArea.Height;
            e.Graphics?.DrawImage(bitmap, 0.0f, 0.0f, height, width);
            e.HasMorePages = false;
        };

        // Handle the EndPrint event to signal task completion after printing
        pd.EndPrint += (sender, e) =>
        {
            tcs.SetResult(true);
        };

        pd.Print();

        await tcs.Task;
    }

    private bool PrintingEnabled()
    {
        return options.Value.PrintingEnabled switch
        {
            PrinterEnabledOptions.True => true,
            PrinterEnabledOptions.False => false,
            PrinterEnabledOptions.Automatic => CheckIfPrinterIsConnected(options.Value.PrinterName), // Example method to check printer status
            _ => false, // Default case, if necessary
        };
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
        using var searcher = new ManagementObjectSearcher(
            $"SELECT Name, PrinterStatus, DetectedErrorState, WorkOffline FROM Win32_Printer WHERE Name LIKE '%{sanitizedPrinterName}%'"
        );

        foreach (var printer in searcher.Get())
        {
            string printerName = printer["Name"]?.ToString()!;

            if (string.Equals(printerName, printerToCheck, StringComparison.OrdinalIgnoreCase))
            {
                // Check if the printer is available and ready to print
                int printerStatus = Convert.ToInt32(printer["PrinterStatus"]); // Status of the printer
                int detectedState = Convert.ToInt32(printer["DetectedErrorState"]); // Error state of the printer
                bool isPrinterOnline = !Convert.ToBoolean(printer["WorkOffline"]); // Checks if the printer is online

                logger.LogInformation(
                    "Printer {PrinterName} with properties printerstatus: {status}, detectedstate {state} and isPrinterOnline {isOnline} found.",
                    printerName,
                    printerStatus,
                    detectedState,
                    isPrinterOnline
                );

                // PrinterStatus 3 means it's idle (ready to print)
                // DetectedErrorState 0 means no error
                // WorkOffline should be false
                if (
                    printerStatus == 3
                    || printerStatus == 2 && detectedState == 0 && isPrinterOnline
                )
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
