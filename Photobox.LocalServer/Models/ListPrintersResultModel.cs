namespace Photobox.LocalServer.Models;

public record ListPrintersResultModel
{
    public string[] Printers { get; set; } = default!;
}
