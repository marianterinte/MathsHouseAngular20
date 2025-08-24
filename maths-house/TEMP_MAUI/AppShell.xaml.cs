using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;

namespace GCMS.MathHouse
{
    public partial class AppShell : Shell
    {
        private readonly GameStateService _gameStateService;
        private readonly VideoManager _videoManager;

        public AppShell(GameStateService gameStateService)
        {
            InitializeComponent();
            _gameStateService = gameStateService;
            _videoManager = new VideoManager();

            // VersionLabel assignment removed, set directly in XAML

            Routing.RegisterRoute("VideoPlayerPage", typeof(VideoPlayerPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
            Routing.RegisterRoute("FreeMathProblemPage", typeof(FreeMathProblemPage));
            
            SetupLanguage();
            SetupNotificationSubscriptions();
        }

        private void SetupNotificationSubscriptions()
        {
            // Subscribe to video end notification
            NotificationService.NotifyVideoEnd += OnVideoEnded;
        }

        private async void OnVideoEnded()
        {
            // Mark that startup video has been played
            PreferencesManager.SetStartupVideoPlayed(true);
            
            // Navigate to MainPage after video completes
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void SetupLanguage()
        {
            var deviceLanguage = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LocalizationService.InitAsync();
                AppTitle.Text = LocalizationService.Translate("app_title");
            });
        }

        public async Task HandleAppStartup()
        {
            // Initialize game state
            _gameStateService.InitializeGameStateService();
            await _gameStateService.LoadAsync();

            // Check if user should be redirected to FreeMathPage
            if (_gameStateService.ShouldRedirectToFreeMathPage())
            {
                await Shell.Current.GoToAsync("//FreeMathPage");
                return;
            }

            // Check if this is a new game and startup video hasn't been played
            if (_gameStateService.IsNewGame() && !PreferencesManager.GetStartupVideoPlayed())
            {
                // Go to startup video first
                await _videoManager.RunStartupVideo();
                return; // Video completion will handle navigation to MainPage
            }

            // Default case: go to MainPage
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void ResetGame(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Resetare joc",
               "Ești sigur că vrei să ștergi tot progresul jocului?",
               "Da, resetează",
               "Nu, păstrează");

            if (answer)
            {
                PreferencesManager.ClearAllExceptLanguage();

                // Resetează starea jocului
                //await _gameStateService.ResetAsync();

                MessagingCenter.Send(this, "GameReset");

                // Redirecționează către pagina principală pentru a reflecta modificările
                await Shell.Current.GoToAsync("//MainPage");
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe to prevent memory leaks
            NotificationService.NotifyVideoEnd -= OnVideoEnded;
        }
    }
}