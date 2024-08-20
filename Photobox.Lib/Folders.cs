namespace Photobox.Lib;

public static class Folders
{
    public static string PhotoboxBaseDir
        => Path.Combine([
            "C:"
            , "Users"
            , Environment.GetEnvironmentVariable("username") ?? ""
            , "Pictures"
            , "Photobox"
            ]);

    public static string Deleted => "Deleted";

    public static string Photos => "Photos";

    public static string Static => "Static";

    public static string Temp => "Temp";

    public static IEnumerable<string> AllFolders
        => [Deleted, Photos, Static, Temp];

    public static string GetPath(string folder)
        => Path.Combine(PhotoboxBaseDir, folder);

    public static string NewImagePath
        => Path.Combine(GetPath(Temp), $"{DateTime.Now:yyyyMMdd_HHmmssfff}.jpg");

    public static void CheckIfDirectoriesExistElseCreate()
    {
        foreach (string path in AllFolders)
        {
            string FullPath = Path.Combine(PhotoboxBaseDir, path);

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
        }
    }

}
