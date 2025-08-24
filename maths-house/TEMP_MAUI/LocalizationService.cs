using GCMS.MathHouse.BL;
using System.Globalization;
using System.Text.Json;

namespace GCMS.MathHouse.Localization;

public static class LocalizationService
{
    private static Dictionary<string, string> _translations = new();
    private const string DefaultLanguage = "ro";

    private static string _currentLanguage = DefaultLanguage;
    private static string _loadedLanguage = "";
    private static bool _isInitialized = false;

    private static readonly string[] SupportedLanguages =
    [
        "ro", "en","hu","es", "de", "fr", "it", "sv", "ar"
    ];

    public static string CurrentLanguage => _currentLanguage;

    public static async Task InitAsync(bool forceReload = false)
    {
        var preferredLang = PreferencesManager.GetLanguage();
        var deviceLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var selectedLang = preferredLang ?? deviceLang;

        if (!SupportedLanguages.Contains(selectedLang))
            selectedLang = "en";

        // 💡 Salvează în preferințe dacă nu era deja salvată
        if (preferredLang == null)
        {
            PreferencesManager.SetLanguage(selectedLang);
        }

        if (!forceReload && _isInitialized && selectedLang == _loadedLanguage)
            return;

        _currentLanguage = selectedLang;
        _loadedLanguage = selectedLang;

        var fileName = $"i18n.{_currentLanguage}.json";

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Could not find or read localization file: {fileName}", ex);
        }
    }


    public static string Translate(string key)
    {
        if (_translations.TryGetValue(key, out var value))
            return value;

        return $"!{key}";
    }
}
