using RestEase;

namespace ImageRunner;

public interface IMjpegClient
{
    [Get]
    Task<Stream> GetStreamAsync();
    
}