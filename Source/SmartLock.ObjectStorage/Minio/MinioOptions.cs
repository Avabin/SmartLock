namespace SmartLock.ObjectStorage.Minio;

public class MinioOptions
{
    public string Endpoint { get; set; } = "localhost";
    public int Port { get; set; } = 9000;
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public bool Secure { get; set; } = false;
}