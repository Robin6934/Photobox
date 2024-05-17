namespace Photobox.Lib.Models;

public class ImageEntryModel(string path, string name)
{
    public string Path { get; init; } = path;

    public string Name { get; init; } = name;
}
