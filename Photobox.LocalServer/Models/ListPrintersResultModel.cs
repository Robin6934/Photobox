namespace Photobox.LocalServer.Models;

public record ListPrintersResultModel
{
    public List<string> Printers { get; set; } = [];
}
