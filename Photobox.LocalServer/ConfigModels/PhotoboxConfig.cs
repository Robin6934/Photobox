using Photobox.LocalServer.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Photobox.LocalServer.ConfigModels;

public class PhotoboxConfig
{
    public static readonly string Photobox = "Photobox";

    public PrinterEnabledOptions PrintingEnabled { get; set; }

    public string PrinterName { get; set; } = default!;

    public void Save()
    {
        string fileName = "appsettings.json";

        string jsonString = File.ReadAllText(fileName);

        JsonObject jsonObject = JsonNode.Parse(jsonString).AsObject();

        JsonObject photoboxJsonObject = JsonSerializer.SerializeToNode(this).AsObject();

        jsonObject[Photobox] = photoboxJsonObject;

        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText(fileName, jsonObject.ToJsonString(options));
    }
}
