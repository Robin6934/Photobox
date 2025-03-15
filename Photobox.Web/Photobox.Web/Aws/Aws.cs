using Amazon.Runtime;
using Amazon.S3;

namespace Photobox.Web.Aws;

public static class Aws
{
    public const string BucketName = "fotos";

    public static void ConfigureAws(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceUrl = configuration["AWS:ServiceURL"];
        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];

        var credentials = new BasicAWSCredentials(accessKey, secretKey);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
            ForcePathStyle = true, // Ensure compatibility with Cloudflare R2
            AuthenticationRegion = "auto",
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
        };

        // Directly register IAmazonS3 with specified config
        services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, s3Config));

        services.AddHealthChecks().AddS3(options =>
        {
            options.BucketName = BucketName;
            options.Credentials = credentials;
            options.S3Config = s3Config;
        });
    }
}
