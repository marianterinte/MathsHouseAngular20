using CommunityToolkit.Maui.Views;

namespace GCMS.MathHouse.UI.Video
{
    public class VideoPlayerUIController
    {
        private MediaElement _mediaElement;
        private MediaElement _audioElement;
        private ImageButton _closeButton;

        public void InitializeUIElements(
            MediaElement mediaElement,
            MediaElement audioElement,
            ImageButton closeButton)
        {
            _mediaElement = mediaElement;
            _audioElement = audioElement;
            _closeButton = closeButton;
        }

        public void UpdateUIBasedOnOptions(VideoPageOptions options)
        {
            if (options == null) return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _closeButton.IsVisible = options.ShowCloseButton;
            });
        }

        public void SubscribeToEvents(Action<string> onMediaFailedAction, Action onMediaEndedAction)
        {
            _mediaElement.MediaFailed += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() => onMediaFailedAction(e.ErrorMessage));
            };

            _mediaElement.MediaEnded += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    //await Task.Delay(1000); // Delay for UI to settle
                    onMediaEndedAction();
                });
            };
        }

        public void PlayMedia()
        {
            _mediaElement?.Play();
            _audioElement?.Play();
        }

        public void PauseMedia()
        {
            _mediaElement?.Pause();
        }

        public void StopMedia()
        {
            _mediaElement?.Stop();
            _audioElement?.Stop();
        }

        public void SetVideoSource(string tempFilePath)
        {
            if (File.Exists(tempFilePath))
            {
                _mediaElement.Source = MediaSource.FromFile(tempFilePath);
            }
        }
    }
}