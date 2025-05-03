using System.Net;
using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.RestApi;

namespace Photobox.Lib.PhotoboxSettingsManager;

public class PhotoboxSettingsManager(IPhotoBoxClient photoBoxClient) : IPhotoboxSettingsManager
{
    public async Task<bool> CheckIfPhotoboxIsRegistered()
    {
        var response = await photoBoxClient.CheckIfPhotoboxExistsAsync();

        return response.Exists;
    }

    public async Task Register(PhotoboxSettings settings)
    {
        var request = new RegisterPhotoBoxDto { PhotoBoxName = settings.Name };

        try
        {
            await photoBoxClient.RegisterAsync(request);
        }
        catch (ApiException e) when (e.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new CredentialValidationException("Invalid credentials.");
        }
    }
}
