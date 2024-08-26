using Microsoft.AspNetCore.Mvc;
using Photobox.LocalServer.Models;
using System.Drawing.Printing;

namespace Photobox.LocalServer.Controllers;
public class PrintController : Controller
{
    public IActionResult ListPrinters()
    {
        ListPrintersResultModel resultModel = new();

        PrinterSettings.InstalledPrinters.CopyTo(resultModel.Printers, 0);

        return Ok(resultModel);
    }
}
