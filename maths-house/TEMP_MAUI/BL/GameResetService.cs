using GCMS.MathHouse.UI.Common;

namespace GCMS.MathHouse.BL;

/// <summary>
/// Centralized service for handling complete game resets
/// </summary>
public class GameResetService
{
    private readonly GameStateService _gameStateService;
    private readonly FreeMathGameFlowController _freeMathController;

    public GameResetService(GameStateService gameStateService, FreeMathGameFlowController freeMathController)
    {
        _gameStateService = gameStateService;
        _freeMathController = freeMathController;
    }

    /// <summary>
    /// Performs a complete game reset including all progress, tutorials, and preferences
    /// </summary>
    public async Task ResetCompleteGameAsync()
    {
        try
        {
            // Reset Part 1: Animal rescue game
            await _gameStateService.ResetAsync();
            
            // Reset Part 2: FreeMath quest
            _freeMathController.ResetProgress();
            
            // Reset tutorial state
            TutorialService.ResetAllTutorials();
            
            // Use the safer clear method to avoid race conditions
            PreferencesManager.SafeClearAllExceptLanguage();
        }
        catch (Exception ex)
        {
            // Log the error but continue - partial reset is better than crash
            System.Diagnostics.Debug.WriteLine($"Error during game reset: {ex.Message}");
            
            // Fallback to individual resets
            try
            {
                await _gameStateService.ResetAsync();
                _freeMathController.ResetProgress();
                TutorialService.ResetAllTutorials();
            }
            catch (Exception fallbackEx)
            {
                System.Diagnostics.Debug.WriteLine($"Fallback reset also failed: {fallbackEx.Message}");
            }
        }
    }

    private void ClearAdditionalGameData()
    {
        try
        {
            PreferencesManager.ClearAdditionalGameData();
        }
        catch (Exception ex)
        {
            // Silent fail - clearing additional data is not critical
            // Could log this if logging service is available
            System.Diagnostics.Debug.WriteLine($"Error clearing additional game data: {ex.Message}");
        }
    }
}