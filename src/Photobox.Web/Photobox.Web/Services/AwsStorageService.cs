using System.Collections.Concurrent;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp.PixelFormats;

namespace Photobox.Web.Services;

public class AwsStorageService(IAmazonS3 amazonS3) : IStorageService
{
    private readonly IAmazonS3 amazonS3 = amazonS3;

    private readonly ConcurrentDictionary<string, Image<Rgb24>> imageBuffer = [];

    public Task StoreImageAsync(
        Image<Rgb24> image,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        imageBuffer.TryAdd(name, image.Clone());

        return UploadImagesAsync(cancellationToken);
    }

    private async Task UploadImagesAsync(CancellationToken cancellationToken)
    {
        foreach (var (imageName, image) in imageBuffer)
        {
            var imageStream = new MemoryStream();

            await image.SaveAsJpegAsync(imageStream, cancellationToken: cancellationToken);

            var request = new PutObjectRequest
            {
                BucketName = Aws.Aws.BucketName,
                DisablePayloadSigning = true,
                Key = imageName,
                InputStream = imageStream,
            };

            await amazonS3.PutObjectAsync(request, cancellationToken);

            imageBuffer.Remove(imageName, out _);
        }
    }

    public async Task<Image<Rgb24>> GetImageAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GetObjectRequest { BucketName = Aws.Aws.BucketName, Key = name };

        if (imageBuffer.TryGetValue(name, out var image))
        {
            return image;
        }
        else
        {
            var response = await amazonS3.GetObjectAsync(request, cancellationToken);

            return await Image.LoadAsync<Rgb24>(response.ResponseStream, cancellationToken);
        }
    }

    public Task DeleteImageAsync(string name, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest() { BucketName = Aws.Aws.BucketName, Key = name };

        return amazonS3.DeleteObjectAsync(request, cancellationToken);
    }

    public Task DeleteImagesAsync(
        IEnumerable<string> images,
        CancellationToken cancellationToken = default
    )
    {
        var request = new DeleteObjectsRequest
        {
            BucketName = Aws.Aws.BucketName,
            Objects = images.Select(k => new KeyVersion { Key = k }).ToList(),
        };

        return amazonS3.DeleteObjectsAsync(request, cancellationToken);
    }

    public Task<string> GetPreSignedUrl(string name, TimeSpan validFor)
    {
        AWSConfigsS3.UseSignatureVersion4 = true;

        var request = new GetPreSignedUrlRequest()
        {
            BucketName = Aws.Aws.BucketName,
            Expires = DateTime.Now.Add(validFor),
            Key = name,
            Verb = HttpVerb.GET,
        };

        return amazonS3.GetPreSignedURLAsync(request);
    }
}
