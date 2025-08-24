using GCMS.MathHouse.UI.Common;
using GCMS.MathHouse.UI.Video;
using System.Text.Json;

namespace GCMS.MathHouse
{
    [QueryProperty(nameof(SerializedOptions), "videoOptions")]
    public partial class VideoPlayerPage : ContentPage
    {
        private readonly VideoPlayerUIController _uiController;
        private readonly VideoPlayerLayoutManager _layoutManager;
        private VideoPageOptions _options;

        public string SerializedOptions
        {
            get => _serializedOptions;
            set
            {
                _serializedOptions = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _options = JsonSerializer.Deserialize<VideoPageOptions>(Uri.UnescapeDataString(value));
                    if (_options != null)
                    {
                        InitializeVideo();
                    }
                }
            }
        }
        private string _serializedOptions;

        public VideoPlayerPage(ResponsiveLayoutService layoutService)
        {
            InitializeComponent();
            _uiController = new VideoPlayerUIController();
            _layoutManager = new VideoPlayerLayoutManager(layoutService);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeUIComponents();
        }

        private void InitializeUIComponents()
        {
            _uiController.InitializeUIElements(mediaElement, audioElement, closeButton);
            _uiController.SubscribeToEvents(
                errorMessage => DisplayAlert("MediaFailed", errorMessage, "OK"),
                OnVideoEnded
            );
            _layoutManager.AdaptLayout(closeButton);
        }

        private void InitializeVideo()
        {
            LoadVideo(_options.VideoFile);
            _uiController.UpdateUIBasedOnOptions(_options);

            if (_options.IsAudioEnabled)
            {
                string audioToPlay = string.IsNullOrEmpty(_options.AudioFile) ? "congrats.mp3" : _options.AudioFile;
                if (_options.AudioFile == "evilwitch.wav")
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        // Așteptăm ca video-ul să se încarce înainte de a reda audio
                        await Task.Delay(2500); // Delay for UI to settle
                        await AudioPlayerService.LoadAndPlayAudio(audioElement, audioToPlay);
                    });

                }
                else
                {
                    AudioPlayerService.LoadAndPlayAudio(audioElement, audioToPlay);
                }
            }
        }

        private async void LoadVideo(string videoFileName)
        {
            try
            {
                using (var assetStream = await FileSystem.OpenAppPackageFileAsync(videoFileName))
                {
                    if (assetStream == null)
                    {
                        await DisplayAlert("Error", $"Could not open asset: {videoFileName}", "OK");
                        return;
                    }

                    var tempFileName = $"{Guid.NewGuid()}_{videoFileName}";
                    var tempFilePath = Path.Combine(FileSystem.CacheDirectory, tempFileName);

                    using (var fileStream = File.OpenWrite(tempFilePath))
                    {
                        await assetStream.CopyToAsync(fileStream);
                    }

                    _uiController.SetVideoSource(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Video Error", ex.Message, "OK");
            }
        }

        private void OnVideoEnded()
        {
            try
            {
                // Notificăm că video-ul s-a terminat folosind NotificationService
                NotificationService.TriggerVideoEnd();

                if (!string.IsNullOrEmpty(_options?.ReturnRoute))
                {
                    OnCloseClicked(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Navigation error: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _uiController.StopMedia();
        }

        private void PlayButton_Clicked(object sender, EventArgs e) => _uiController.PlayMedia();
        private void PauseButton_Clicked(object sender, EventArgs e) => _uiController.PauseMedia();
        private void StopButton_Clicked(object sender, EventArgs e) => _uiController.StopMedia();

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            try
            {
                _uiController.StopMedia();
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Navigation error: {ex.Message}");
            }
        }
    }
}