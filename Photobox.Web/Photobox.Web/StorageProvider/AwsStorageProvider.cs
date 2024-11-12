using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.StorageProvider;

public class AwsStorageProvider(IAmazonS3 amazonS3) : IStorageProvider
{
    private readonly IAmazonS3 amazonS3 = amazonS3;

    public async Task StoreImageAsync(Image<Rgb24> image, string name)
    {
        var imageStream = new MemoryStream();

        await image.SaveAsJpegAsync(imageStream);

        var request = new PutObjectRequest
        {
            BucketName = "fotos",
            DisablePayloadSigning = true,
            Key = name,
            InputStream = imageStream
        };

        await amazonS3.PutObjectAsync(request);
    }

    public async Task<Image<Rgb24>> GetImageAsync(string name)
    {
        var request = new GetObjectRequest
        {
            BucketName = "fotos",
            Key = name,
        };

        var response = await amazonS3.GetObjectAsync(request);

        return await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(response.ResponseStream);
    }

    public Task DeleteImageAsync(string name)
    {
        var request = new DeleteObjectRequest()
        {
            BucketName = "fotos",
            Key = name
        };

        return amazonS3.DeleteObjectAsync(request);
    }
}
