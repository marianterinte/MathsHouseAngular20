using CommunityToolkit.Maui.Views;

namespace GCMS.MathHouse.UI.Common;
public class AudioManager
{
    public async Task PlayMainMusic(MediaElement audioElement)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await AudioPlayerService.LoadAndPlayAudio(audioElement, "mainMusic.wav");
        });
    }

    public async Task PlayMainPageMusic(MediaElement audioElement)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await AudioPlayerService.LoadAndPlayAudio(audioElement, "mainPageMusic.wav");
        });
    }

    public async Task PlayFreeMathPageMusic(MediaElement audioElement)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await AudioPlayerService.LoadAndPlayAudio(audioElement, "freeMathPageMusic.wav");
        });
    }

    public async Task StopAudio(MediaElement audioElement)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await AudioPlayerService.StopAudio(audioElement);
        });
    }
}

