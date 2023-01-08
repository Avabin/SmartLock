namespace Webcam;

public interface IWebcamDevice
{
    IWebcamChannel GetChannel(int channel);
}