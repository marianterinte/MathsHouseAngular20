using System.Text.Json;

namespace GCMS.MathHouse.UI.Common
{
    public class VideoManager
    {
        public async Task RunStartupVideo()
        {
            await GoToVideoPage("ZStartingVideo.mp4",
                "//MainPage",
                isAudioEnabled: true,
                showCloseButton: false,
                audioFile: "evilwitch.wav");
        }

        public async Task PlayAnimalVideo(string floorId)
        {
            if (floorId == "TopFloor")
            {
                await GoToVideoPage("ZEndVideo.mp4",
                    "//MainPage",
                    isAudioEnabled: true,
                    showCloseButton: true);
            }
            else
            {
                string fileName = $"{floorId}.mp4";
                await GoToVideoPage(fileName);
            }
        }

        private static async Task GoToVideoPage(string fileName, string returnPage = "", bool isAudioEnabled = true, bool showCloseButton = true, string audioFile = "")
        {
            var options = new VideoPageOptions
            {
                VideoFile = fileName,
                AudioFile = audioFile,
                ReturnRoute = returnPage,
                Message = "Bravo!",
                IsAudioEnabled = isAudioEnabled,
                ShowTileText = false,
                ShowContinueButton = false,
                ShowCloseButton = showCloseButton
            };

            string json = JsonSerializer.Serialize(options);
            string encodedOptions = Uri.EscapeDataString(json);

            await Shell.Current.GoToAsync($"//VideoPlayerPage?videoOptions={encodedOptions}");
        }
    }
}