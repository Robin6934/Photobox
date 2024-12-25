using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Photobox.Web.StorageProvider;

public class AwsStorageProvider(IAmazonS3 amazonS3) : IStorageProvider
{
    private readonly IAmazonS3 amazonS3 = amazonS3;

    private readonly ConcurrentDictionary<string, Image<Rgb24>> imageBuffer = [];

    public Task StoreImageAsync(Image<Rgb24> image, string name)
    {
        imageBuffer.TryAdd(name, image);

        return UploadImagesAsync();
    }

    private async Task UploadImagesAsync()
    {
        foreach (var (imageName, image) in imageBuffer)
        {
            var imageStream = new MemoryStream();

            await image.SaveAsJpegAsync(imageStream);

            var request = new PutObjectRequest
            {
                BucketName = Aws.Aws.BucketName,
                DisablePayloadSigning = true,
                Key = imageName,
                InputStream = imageStream
            };

            try
            {
                await amazonS3.PutObjectAsync(request);

                imageBuffer.Remove(imageName, out _);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public async Task<Image<Rgb24>> GetImageAsync(string name)
    {
        var request = new GetObjectRequest
        {
            BucketName = Aws.Aws.BucketName,
            Key = name
        };

        if (imageBuffer.TryGetValue(name, out var image))
        {
            return image;
        }
        else
        {
            var response = await amazonS3.GetObjectAsync(request);

            return await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(response.ResponseStream);
        }
    }

    public Task DeleteImageAsync(string name)
    {
        var request = new DeleteObjectRequest()
        {
            BucketName = Aws.Aws.BucketName,
            Key = name
        };

        return amazonS3.DeleteObjectAsync(request);
    }

    public string GetPreSignedUrl(string name, TimeSpan validFor)
    {
        AWSConfigsS3.UseSignatureVersion4 = true;

        var request = new GetPreSignedUrlRequest()
        {
            BucketName = Aws.Aws.BucketName,
            Expires = DateTime.Now.Add(validFor),
            Key = name,
            Verb = HttpVerb.GET,
        };

        return amazonS3.GetPreSignedURL(request);
    }
}
