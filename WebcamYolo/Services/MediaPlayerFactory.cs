using System;
using LibVLCSharp.Shared;

namespace WebcamYolo.Services;

public class MediaPlayerFactory
{
    private Lazy<LibVLC> _vlc;
    public LibVLC Vlc => _vlc.Value;

    public MediaPlayerFactory()
    {
        var options = new VlcOptions();
        _vlc = new Lazy<LibVLC>(() => new LibVLC(options.Path));
    }
    
    public MediaPlayer CreateMediaPlayer() => new(Vlc);
    
    public Media CreateMedia(string murl, params string[] options) => new(Vlc, murl, options:options);
}