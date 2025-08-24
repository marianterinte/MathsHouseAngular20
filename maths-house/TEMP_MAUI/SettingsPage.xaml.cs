using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;
using Microsoft.Maui.Controls.PlatformConfiguration;


#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace GCMS.MathHouse;

public partial class SettingsPage : ContentPage
{
    private readonly GameStateService _gameStateService;
    private readonly GameResetService _gameResetService;
    private bool _isInitializingLanguage = false;

    private readonly List<string> _languageNames = new()
    {
        "🇷🇴 Română",
        "🇬🇧 English",
        "🇭🇺 Magyar",
        "🇪🇸 Español",
        "🇩🇪 Deutsch",
        "🇫🇷 Français",
        "🇮🇹 Italiano",
        "🇸🇪 Svenska",
        "🇸🇦 العربية"
    };

    private readonly Dictionary<string, int> _langIndexMap = new()
    {
        { "ro", 0 },
        { "en", 1 },
        { "hu", 2 },
        { "es", 3 },
        { "de", 4 },
        { "fr", 5 },
        { "it", 6 },
        { "sv", 7 },
        { "ar", 8 },
    };

    public SettingsPage(GameStateService gameStateService, GameResetService gameResetService)
    {
        _gameStateService = gameStateService;
        _gameResetService = gameResetService;
        InitializeComponent();

        // Afișează UI în limba curentă la pornire
        MainThread.BeginInvokeOnMainThread(async () => await SetupLanguage());
    }

    private async Task SetupLanguage()
    {
        _isInitializingLanguage = true;
        await LocalizationService.InitAsync(); // încarcă fișierul JSON

        ContinueButton.Text = LocalizationService.Translate("settings_continue");
        NewGameButton.Text = LocalizationService.Translate("settings_new_game");
        AboutButton.Text = LocalizationService.Translate("settings_about");
        ExitButton.Text = LocalizationService.Translate("settings_exit");

        // Traducere titlu picker
        LanguagePicker.Title = LocalizationService.Translate("settings_language_label");

        // Setăm limba în picker
        LanguagePicker.ItemsSource = _languageNames;

        var currentLang = LocalizationService.CurrentLanguage;
        LanguagePicker.SelectedIndex = _langIndexMap.TryGetValue(currentLang, out int index) ? index : 1;
        _isInitializingLanguage = false;
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        if (_isInitializingLanguage)
            return; // Ignoră evenimentul când faci setup

        var selected = LanguagePicker.SelectedIndex;

        string newLang = selected switch
        {
            0 => "ro",
            1 => "en",
            2 => "hu",
            3 => "es",
            4 => "de",
            5 => "fr",
            6 => "it",
            7 => "sv",
            8 => "ar",
            _ => "en"
        };

        PreferencesManager.SetLanguage(newLang);

        await LocalizationService.InitAsync(forceReload: true);
        await SetupLanguage();

        RestartApp();
    }

    private async void OnNewGameClicked(object sender, EventArgs e)
    {
        var title = LocalizationService.Translate("settings_new_game_title");
        var question = LocalizationService.Translate("settings_new_game_question");
        var optionYes = LocalizationService.Translate("settings_new_game_yes");
        var optionNo = LocalizationService.Translate("settings_new_game_no");

        bool confirm = await DisplayAlert(title, question, optionYes, optionNo);
        if (confirm)
        {
            // Use centralized reset service to reset everything
            await _gameResetService.ResetCompleteGameAsync();
            
            // Restart the application for clean UI reset
            RestartApp();
        }
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        await _gameStateService.NavigateToHomePage();
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AboutPage");
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
#if ANDROID
        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
        System.Environment.Exit(0);
#endif
    }

    private void RestartApp()
    {
        App.Current.MainPage = new AppShell(_gameStateService);
    }
}
