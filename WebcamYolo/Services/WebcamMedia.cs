namespace WebcamYolo.Services;

public class VlcWebcamMedia
{
    const string url = "dshow://video=Integrated Webcam";
    // set aspect ratio to 16:9
    const string args = ":dshowsize=1920x1080";

    public VlcWebcamMedia()
    {
        
    }
}