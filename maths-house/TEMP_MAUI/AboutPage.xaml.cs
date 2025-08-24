using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse;

public partial class AboutPage : ContentPage
{
    private const string RevolutIban = "RO52REVO0000195759253188"; 
    private const string SaltIban = "RO13ROIN40211VRATQ9KOYYM"; 
    // Example IBAN
    private readonly GameStateService _gameStateService;

    public AboutPage(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SetupLanguageTranslations();
    }

    private async Task SetupLanguageTranslations()
    {
        await LocalizationService.InitAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Title = LocalizationService.Translate("about_page_title");
            
            // Set MENTAI button text
            MentaiLinkButton.Text = LocalizationService.Translate("about_page_mentai_link");
            
            DescriptionLabel.Text = LocalizationService.Translate("about_page_description");
            SupportMessageLabel.Text = LocalizationService.Translate("about_page_support_message");

            // Hardcoded IBANs
            RevolutLabel.Text = $"Revolut IBAN: {RevolutIban}";
            SaltLabel.Text = $"Salt IBAN: {SaltIban}";
           
            SaltBankLabel.Text = LocalizationService.Translate("about_page_salt_bank");
            ContactMessageLabel.Text = LocalizationService.Translate("about_page_contact_message");
            EmailLabel.Text = LocalizationService.Translate("about_page_email");
            FutureNotesTitle.Text = LocalizationService.Translate("about_page_future_notes_title");
            FutureFeature1.Text = LocalizationService.Translate("about_page_future_feature_1");
            FutureFeature2.Text = LocalizationService.Translate("about_page_future_feature_2");
            FutureFeature3.Text = LocalizationService.Translate("about_page_future_feature_3");

        });
    }

    private async void OnMentaiLinkClicked(object sender, EventArgs e)
    {
        try
        {
            await Browser.OpenAsync("https://study.withmentai.com/", BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            // Handle exception if browser fails to open
            await DisplayAlert("Error", "Could not open browser", "OK");
        }
    }

    private async void OnCopyRevolutClicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(RevolutIban);
        await Shell.Current.DisplayAlert("Copied", "Revolut IBAN copied to clipboard.", "OK");
    }

    private async void OnCopySaltClicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(SaltIban);
        await Shell.Current.DisplayAlert("Copied", "Salt IBAN copied to clipboard.", "OK");
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await _gameStateService.NavigateToHomePage();
    }
}
