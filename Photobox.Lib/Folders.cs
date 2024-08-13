namespace Photobox.Lib;

public static class Folders
{
    public static string PhotoBoothBaseDir
        => Path.Combine([
            "C:"
            , "Users"
            , Environment.GetEnvironmentVariable("username") ?? ""
            , "Pictures"
            , "Photobox"
            ]);

    public static string Deleted => "Deleted";

    public static string Photos => "Photos";

    public static string ShowTemp => "ShowTemp";

    public static string Static => "Static";

    public static string Temp => "Temp";

    public static IEnumerable<string> AllFolders
        => [Deleted, Photos, ShowTemp, Static, Temp];

    public static string GetPath(string folder)
        => Path.Combine(PhotoBoothBaseDir, folder);

    public static string NewImagePath
        => Path.Combine(GetPath(Temp), $"{DateTime.Now:yyyyMMdd_HHmmssfff}.jpg");

}
