using System.Text.Json;

namespace GCMS.MathHouse.BL;

/// <summary>
/// Centralized manager for all application preferences and settings
/// Encapsulates all preference keys and provides type-safe access to stored values
/// </summary>
public static class PreferencesManager
{
    // Game Progress Keys
    private const string GameProgressKey = "GameProgress";

    // FreeMath Quest Keys  
    private const string FreeMathProgressKey = "FreeMathProgress";
    private const string FreeMathCollectedIngredientsKey = "FreeMathCollectedIngredients";
    private const string HasReachedPart2Key = "HasReachedPart2";

    // Tutorial Keys
    private const string CharacterInteractionTutorialKey = "CharacterInteractionTutorialShown";

    // Video Keys
    private const string StartupVideoPlayedKey = "StartupVideoPlayed";

    // Localization Keys
    private const string LanguageKey = "language";

    // Additional Game Data Keys
    private const string FirstTimeUserKey = "FirstTimeUser";
    private const string LastPlayedDateKey = "LastPlayedDate";
    private const string SessionCountKey = "SessionCount";
    private const string GameCompletionTimeKey = "GameCompletionTime";
    private const string UserPreferencesKey = "UserPreferences";

    private static string CurrentIngredientKey = "CurrentIngredient";

    // Game Progress Methods
    public static string GetGameProgress(string defaultValue = "")
    {
        return Preferences.Get(GameProgressKey, defaultValue);
    }

    public static void SetGameProgress(string value)
    {
        Preferences.Set(GameProgressKey, value);
    }

    public static bool HasGameProgress()
    {
        return Preferences.ContainsKey(GameProgressKey);
    }

    public static void RemoveGameProgress()
    {
        Preferences.Remove(GameProgressKey);
    }

    // FreeMath Progress Methods
    public static int GetFreeMathProgress(int defaultValue = 0)
    {
        return Preferences.Get(FreeMathProgressKey, defaultValue);
    }

    public static void SetFreeMathProgress(int value)
    {
        Preferences.Set(FreeMathProgressKey, value);
    }

    // FreeMath Collected Ingredients Methods
    public static string GetFreeMathCollectedIngredients(string defaultValue = "[]")
    {
        return Preferences.Get(FreeMathCollectedIngredientsKey, defaultValue);
    }

    public static List<IngredientType> GetCollectedIngredients()
    {
        try
        {
            var jsonString = GetFreeMathCollectedIngredients("[]");

            var ingredients = JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            return ingredients.ConvertAll(i => (IngredientType)Enum.Parse(typeof(IngredientType), i));
        }
        catch (Exception ex)
        {
            return new List<IngredientType>();
        }
    }


    public static void SetFreeMathCollectedIngredients(string value)
    {
        Preferences.Set(FreeMathCollectedIngredientsKey, value);
    }

    public static void ClearFreeMathCollectedIngredients()
    {
        Preferences.Set(FreeMathCollectedIngredientsKey, "[]");
    }

    // Startup Video Methods
    public static bool GetStartupVideoPlayed(bool defaultValue = false)
    {
        return Preferences.Get(StartupVideoPlayedKey, defaultValue);
    }

    public static void SetStartupVideoPlayed(bool value)
    {
        Preferences.Set(StartupVideoPlayedKey, value);
    }

    public static void RemoveStartupVideoPlayed()
    {
        Preferences.Remove(StartupVideoPlayedKey);
    }

    // Tutorial Methods
    public static bool IsCharacterInteractionTutorialShown(bool defaultValue = false)
    {
        return Preferences.Get(CharacterInteractionTutorialKey, defaultValue);
    }

    public static void SetCharacterInteractionTutorialShown(bool value)
    {
        Preferences.Set(CharacterInteractionTutorialKey, value);
    }

    public static void RemoveCharacterInteractionTutorial()
    {
        Preferences.Remove(CharacterInteractionTutorialKey);
    }

    // Language Methods
    public static string GetLanguage(string defaultValue = null)
    {
        return Preferences.Get(LanguageKey, defaultValue);
    }

    public static void SetLanguage(string value)
    {
        Preferences.Set(LanguageKey, value);
    }

    // Additional Game Data Methods
    public static bool GetFirstTimeUser(bool defaultValue = true)
    {
        return Preferences.Get(FirstTimeUserKey, defaultValue);
    }

    public static void SetFirstTimeUser(bool value)
    {
        Preferences.Set(FirstTimeUserKey, value);
    }

    public static DateTime? GetLastPlayedDate()
    {
        var dateString = Preferences.Get(LastPlayedDateKey, null);
        return dateString != null && DateTime.TryParse(dateString, out var date) ? date : null;
    }

    public static void SetLastPlayedDate(DateTime value)
    {
        Preferences.Set(LastPlayedDateKey, value.ToString("O"));
    }

    public static int GetSessionCount(int defaultValue = 0)
    {
        return Preferences.Get(SessionCountKey, defaultValue);
    }

    public static void SetSessionCount(int value)
    {
        Preferences.Set(SessionCountKey, value);
    }

    public static TimeSpan? GetGameCompletionTime()
    {
        var timeString = Preferences.Get(GameCompletionTimeKey, null);
        return timeString != null && TimeSpan.TryParse(timeString, out var time) ? time : null;
    }

    public static void SetGameCompletionTime(TimeSpan value)
    {
        Preferences.Set(GameCompletionTimeKey, value.ToString());
    }

    public static string GetUserPreferences(string defaultValue = "{}")
    {
        return Preferences.Get(UserPreferencesKey, defaultValue);
    }

    public static void SetUserPreferences(string value)
    {
        Preferences.Set(UserPreferencesKey, value);
    }

    // Utility Methods
    public static bool ContainsKey(string key)
    {
        return key switch
        {
            GameProgressKey => Preferences.ContainsKey(GameProgressKey),
            FreeMathProgressKey => Preferences.ContainsKey(FreeMathProgressKey),
            FreeMathCollectedIngredientsKey => Preferences.ContainsKey(FreeMathCollectedIngredientsKey),
            CharacterInteractionTutorialKey => Preferences.ContainsKey(CharacterInteractionTutorialKey),
            StartupVideoPlayedKey => Preferences.ContainsKey(StartupVideoPlayedKey),
            LanguageKey => Preferences.ContainsKey(LanguageKey),
            FirstTimeUserKey => Preferences.ContainsKey(FirstTimeUserKey),
            LastPlayedDateKey => Preferences.ContainsKey(LastPlayedDateKey),
            SessionCountKey => Preferences.ContainsKey(SessionCountKey),
            GameCompletionTimeKey => Preferences.ContainsKey(GameCompletionTimeKey),
            UserPreferencesKey => Preferences.ContainsKey(UserPreferencesKey),
            _ => false
        };
    }

    public static void RemoveKey(string key)
    {
        switch (key)
        {
            case GameProgressKey:
                RemoveGameProgress();
                break;
            case FreeMathProgressKey:
                Preferences.Remove(FreeMathProgressKey);
                break;
            case FreeMathCollectedIngredientsKey:
                ClearFreeMathCollectedIngredients();
                break;
            case CharacterInteractionTutorialKey:
                RemoveCharacterInteractionTutorial();
                break;
            case StartupVideoPlayedKey:
                RemoveStartupVideoPlayed();
                break;
            case LanguageKey:
                Preferences.Remove(LanguageKey);
                break;
            case FirstTimeUserKey:
                Preferences.Remove(FirstTimeUserKey);
                break;
            case LastPlayedDateKey:
                Preferences.Remove(LastPlayedDateKey);
                break;
            case SessionCountKey:
                Preferences.Remove(SessionCountKey);
                break;
            case GameCompletionTimeKey:
                Preferences.Remove(GameCompletionTimeKey);
                break;
            case UserPreferencesKey:
                Preferences.Remove(UserPreferencesKey);
                break;
        }
    }

    /// <summary>
    /// Reset all tutorials
    /// </summary>
    public static void ResetAllTutorials()
    {
        RemoveCharacterInteractionTutorial();
    }

    /// <summary>
    /// Clear all additional game data (used in game reset)
    /// </summary>
    public static void ClearAdditionalGameData()
    {
        try
        {
            var additionalKeys = new[]
            {
                FirstTimeUserKey,
                LastPlayedDateKey,
                SessionCountKey,
                GameCompletionTimeKey,
                UserPreferencesKey
            };

            foreach (var key in additionalKeys)
            {
                if (ContainsKey(key))
                {
                    RemoveKey(key);
                }
            }
        }
        catch (Exception ex)
        {
            // Silent fail - clearing additional data is not critical
            // Could log this if logging service is available
        }
    }

    /// <summary>
    /// Clear all preferences while preserving language setting
    /// This method safely clears all preferences without causing state inconsistencies
    /// </summary>
    public static void ClearAllExceptLanguage()
    {
        try
        {
            // Save the language preference BEFORE clearing anything
            var preferredLang = GetLanguage();

            // Clear all game-specific preferences one by one to avoid race conditions
            RemoveGameProgress();
            Preferences.Remove(FreeMathProgressKey);
            ClearFreeMathCollectedIngredients();
            RemoveHasReachedPart2();
            RemoveCharacterInteractionTutorial();
            RemoveStartupVideoPlayed();

            // Clear additional game data
            ClearAdditionalGameData();

            // Restore language setting
            if (preferredLang != null)
            {
                SetLanguage(preferredLang);
            }
        }
        catch (Exception ex)
        {
            // If there's an error, fallback to the original method but log it
            System.Diagnostics.Debug.WriteLine($"Error in ClearAllExceptLanguage: {ex.Message}");

            // Fallback: preserve language and clear everything else
            var preferredLang = GetLanguage();
            Preferences.Clear();
            if (preferredLang != null)
            {
                SetLanguage(preferredLang);
            }
        }
    }

    /// <summary>
    /// Safe method to clear all preferences individually (alternative to ClearAllExceptLanguage)
    /// This avoids using Preferences.Clear() which can cause race conditions
    /// </summary>
    public static void SafeClearAllExceptLanguage()
    {
        try
        {
            // Save the language preference
            var preferredLang = GetLanguage();

            // Remove all keys individually instead of using Preferences.Clear()
            var allKeys = new[]
            {
                GameProgressKey,
                FreeMathProgressKey,
                FreeMathCollectedIngredientsKey,
                HasReachedPart2Key,
                CharacterInteractionTutorialKey,
                StartupVideoPlayedKey,
                FirstTimeUserKey,
                LastPlayedDateKey,
                SessionCountKey,
                GameCompletionTimeKey,
                UserPreferencesKey
            };

            foreach (var key in allKeys)
            {
                if (key != LanguageKey && Preferences.ContainsKey(key))
                {
                    Preferences.Remove(key);
                }
            }

            // Restore language setting
            if (preferredLang != null)
            {
                SetLanguage(preferredLang);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in SafeClearAllExceptLanguage: {ex.Message}");
        }
    }

    // HasReachedPart2 Methods (for marking when user first reaches FreeMathPage)
    public static bool GetHasReachedPart2(bool defaultValue = false)
    {
        return Preferences.Get(HasReachedPart2Key, defaultValue);
    }

    public static void SetHasReachedPart2(bool value)
    {
        Preferences.Set(HasReachedPart2Key, value);
    }

    public static void RemoveHasReachedPart2()
    {
        Preferences.Remove(HasReachedPart2Key);
    }

    public static void SetCurrentIngredient(IngredientType ingredient)
    {
        Preferences.Set(CurrentIngredientKey, ingredient.ToString());
    }

    public static IngredientType GetCurrentIngredient(IngredientType defaultValue = IngredientType.CarrotDust)
    {
        var ingredientString = Preferences.Get(CurrentIngredientKey, defaultValue.ToString());
        return Enum.TryParse(ingredientString, out IngredientType ingredient) ? ingredient : defaultValue;
    }
}