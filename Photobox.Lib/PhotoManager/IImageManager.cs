namespace Photobox.Lib.PhotoManager;
public interface IImageManager
{
    public Task PrintAndSaveAsync(string imagePath);

    public void Save(string imagePath);

    public void Delete(string imagePath);
}
