namespace Photobox.Lib.PhotoboxSettingsManager;

public interface IPhotoboxSettingsManager
{
    /// <summary>
    /// Checks if the current photobox id is registered on the server under the current user.
    /// </summary>
    /// <returns>Whether the current photobox id already exists</returns>
    public Task<bool> CheckIfPhotoboxIsRegistered();

    public Task Register(PhotoboxSettings settings);
}
