namespace Photobox.UI.Lib;

public static class Folders
{
    /// <summary>
    /// Gets executed, when the class is used for the first time, only once.
    /// </summary>
    static Folders()
    {
        CheckIfDirectoriesExistElseCreate();
    }

    public static string PhotoboxBaseDir
        => Path.Combine([Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            , "Pictures"
            , "Photobox"
            ]);

    public static string Deleted => "Deleted";

    public static string Photos => "Photos";

    public static string Temp => "Temp";

    public static IEnumerable<string> AllFolders
        => [Deleted, Photos, Temp];

    public static string GetPath(params string[] path)
        => Path.Combine(PhotoboxBaseDir, Path.Combine(path));

    public static string NewImagePath
        => Path.Combine(GetPath(Temp), NewImageName);

    public static string NewImageName
        => $"{DateTime.Now:yyyyMMdd_HHmmssfff}.jpg";

    public static void CheckIfDirectoriesExistElseCreate()
    {
        foreach (string path in AllFolders)
        {
            string FullPath = GetPath(path);

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
        }
    }

}
