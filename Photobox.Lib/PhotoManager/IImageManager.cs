namespace Photobox.Lib.PhotoManager;
public interface IImageManager
{
    public Task PrintAndSaveAsync(string imagePath, string printerName);

    public void Save(string imagePath);

    public void Delete(string imagePath);
}
