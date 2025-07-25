﻿using System.Text.Json;
using System.Text.Json.Nodes;
using Photobox.UI.Lib.Camera;
using Photobox.UI.Lib.Printer;

namespace Photobox.UI.Lib.ConfigModels;

public class PhotoboxConfig
{
    public static readonly string Photobox = "Photobox";

    public string ServerUrl { get; set; }

    public PrinterEnabledOptions PrintingEnabled { get; set; }

    public string PrinterName { get; set; } = default!;

    public CameraType Camera { get; set; }

    public CountDownConfig CountDown { get; set; } = default!;

    public string TextOnPicture { get; set; } = "";

    public bool StoreDeletedImages { get; set; }

    public bool AutoOff { get; set; }

    public void Save()
    {
        string fileName = "appsettings.json";

        string jsonString = File.ReadAllText(fileName);

        JsonObject jsonObject = JsonNode.Parse(jsonString)!.AsObject();

        JsonObject photoBoxJsonObject = JsonSerializer.SerializeToNode(this)!.AsObject();

        jsonObject[Photobox] = photoBoxJsonObject;

        JsonSerializerOptions options = new() { WriteIndented = true };

        File.WriteAllText(fileName, jsonObject.ToJsonString(options));
    }
}
