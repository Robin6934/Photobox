namespace Photobox.Lib;

public static class Folders
{
    public static string PhotoBoothBaseDir { get => Path.Combine(["C:", "Users", Environment.GetEnvironmentVariable("username") ?? "", "Pictures", "Photobox"]); }

    public static string Deleted { get => "Deleted"; }

    public static string Photos { get => "Photos"; }

    public static string ShowTemp { get => "ShowTemp"; }

    public static string Static { get => "Static"; }

    public static string Temp { get => "Temp"; }

    public static IEnumerable<string> AllFolders { get => [Deleted, Photos, ShowTemp, Static, Temp]; }

    public static string GetPath(string folder) => Path.Combine(PhotoBoothBaseDir, folder);
}
