using Photobox.Lib.AccessTokenManager;
using Photobox.Lib.RestApi;

namespace Photobow.Web.Uploader;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new ImageClient("https://localhost");

        ImageClient.AccessTokenManager = new AccessTokenManager(new Client("https://localhost"));
        await ImageClient.AccessTokenManager.LoginAsync("Test@gmail.com", "Test@1234");

        foreach (var image in Directory.EnumerateFiles(@"E:\Photos\19.07.22_Bach", "*.JPG", SearchOption.TopDirectoryOnly))
        {
            var formFile = new FileParameter(File.OpenRead(image), Path.GetFileName(image));
            
            await client.UploadImageAsync(formFile);
        }
    }
}