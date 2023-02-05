using Microsoft.Extensions.Logging;
using Minio;

namespace SmartLock.ObjectStorage.Minio;

public class MinioObjectStorage : IObjectStorage
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioObjectStorage> _logger;

    public MinioObjectStorage(IMinioClient client, ILogger<MinioObjectStorage> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<string> UploadAsync(string bucketName, string objectName, Stream data, string contentType,long size)
    {
        _logger.LogInformation("Uploading object {ObjectName} to bucket {BucketName}", objectName, bucketName);
        // check if bucket exists
        var bucketExists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!bucketExists)
        {
            _logger.LogInformation("Bucket {BucketName} does not exist, creating it", bucketName);
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
        _logger.LogDebug("Uploading object {ObjectName} to bucket {BucketName}", objectName, bucketName);
        await _client.PutObjectAsync(new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(data).WithObjectSize(size).WithContentType(contentType));
        
        _logger.LogDebug("Generating presigned url for object {ObjectName} in bucket {BucketName}", objectName, bucketName);
        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(3600));
        _logger.LogTrace("Generated presigned url {Url} for object {ObjectName} in bucket {BucketName}", url, objectName, bucketName);
        
        return url ?? throw new Exception("Failed to generate presigned url");
    }

    public async Task DeleteAsync(string bucketName, string fileName)
    {
        _logger.LogInformation("Deleting object {FileName} from bucket {BucketName}", fileName, bucketName);
        // check if bucket exists
        var bucketExists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!bucketExists)
        {
            _logger.LogInformation("Bucket {BucketName} does not exist, creating it", bucketName);
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
        
        // check if object exists
        _logger.LogDebug("Checking if object {FileName} exists in bucket {BucketName}", fileName, bucketName);
        var objectStat = await _client.StatObjectAsync(new StatObjectArgs().WithBucket(bucketName).WithObject(fileName));
        if (objectStat != null)
        {
            _logger.LogDebug("Object {FileName} exists in bucket {BucketName}, deleting it", fileName, bucketName);
            await _client.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucketName).WithObject(fileName));
            _logger.LogTrace("Object {FileName} deleted from bucket {BucketName}", fileName, bucketName);
        }
    }
}