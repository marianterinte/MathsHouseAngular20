namespace GCMS.MathHouse;
public class VideoPageOptions
{
    public string VideoFile { get; set; }
    public string AudioFile { get; set; }
    public string ReturnRoute { get; set; }
    public string Message { get; set; }
    public bool IsAudioEnabled { get; set; } = true;
    public bool ShowTileText { get; set; } = false;
    public bool ShowContinueButton { get; set; } = false;
    public bool ShowCloseButton { get; set; } = true;
}
