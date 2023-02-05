namespace SmartLock.ObjectStorage;

public interface IObjectStorage
{
    Task<string> UploadAsync(string bucketName, string objectName, Stream data, string contentType, long size);
    Task DeleteAsync(string bucketName, string fileName);
}