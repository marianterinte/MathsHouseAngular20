using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;
using GCMS.MathHouse.UI.MainPage;

namespace GCMS.MathHouse;

[QueryProperty(nameof(Success), "success")]
public partial class FreeMathPage : ContentPage
{
    private readonly FreeMathGameFlowController _flowController;
    private readonly FreeMathLayoutManager _layoutManager;
    private readonly TypewriterAnimationService _typewriterService;
    private readonly TutorialPopupManager _tutorialPopupManager;
    private readonly GameFlowController _mainGameFlowController;
    private readonly AudioManager _audioManager;

    private bool _success;

    private string _fullNarrativeText = "";
    private bool _isCharacterInteractive = false;
    private bool _isUpdatingStep = false;
    private bool _queryPropertiesProcessed = false;
    private bool _tutorialShown = false;
    private bool _hasNewProgressUpdate = false;

    private CancellationTokenSource _glowAnimationCancellation;

    public bool Success
    {
        get => _success;
        set
        {
            _success = value;
            _queryPropertiesProcessed = true;

            if (_success)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await HandleProblemSolved();
                });
            }
        }
    }

    public FreeMathPage(GameStateService gameStateService, ResponsiveLayoutService layoutService,
                        FreeMathGameFlowController flowController, GameFlowController mainGameFlowController)
    {
        InitializeComponent();

        _flowController = flowController;
        _mainGameFlowController = mainGameFlowController;
        _layoutManager = new FreeMathLayoutManager(layoutService);
        _typewriterService = new TypewriterAnimationService();
        _tutorialPopupManager = new TutorialPopupManager();
        _audioManager = new AudioManager();

        _typewriterService.AnimationStateChanged += OnAnimationStateChanged;
    }

    private async Task InitializeAsync()
    {
        await Task.Run(async () =>
        {
            await LocalizationService.InitAsync();
            await _layoutManager.InitializeAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                AdaptLayout();

                var ingredientCount = _flowController.GetCollectedIngredientsCount();

                UpdateProgressCard();
            });
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Start playing FreeMathPage music when entering the page
        await _audioManager.PlayFreeMathPageMusic(audioElement);

        if (!PreferencesManager.GetHasReachedPart2())
        {
            PreferencesManager.SetHasReachedPart2(true);
        }

        // Initialize async components first
        await InitializeAsync();

        await SetupLanguageTranslations();

        _flowController.RefreshStepsWithCurrentLocalization();

        _flowController.LoadProgress();

        var ingredientCount = _flowController.GetCollectedIngredientsCount();
        var collectedIngredients = _flowController.GetCollectedIngredients();

        UpdateProgressCard();

        // Show tutorial immediately if needed, BEFORE updating the current step
        if (!_tutorialShown && TutorialService.ShouldShowCharacterInteractionTutorial())
        {
            _tutorialShown = true;
            await _tutorialPopupManager.ShowCharacterInteractionTutorial();
        }

        // Always update current step and play animation
        if (!_queryPropertiesProcessed && !_isUpdatingStep)
        {
            await UpdateCurrentStep();
        }

        _queryPropertiesProcessed = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _typewriterService?.StopAnimation();
        _glowAnimationCancellation?.Cancel();
        _queryPropertiesProcessed = false;
        // Pause audio when leaving FreeMathPage
        StopMusicSafely();
    }

    private void StopMusicSafely()
    {
        try
        {
            _ = _audioManager.StopAudio(audioElement);
        }
        catch (Exception ex)
        {
            // Silent fail - audio stopping error
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler == null)
        {
            CleanupAnimationService();
        }
    }

    private void CleanupAnimationService()
    {
        if (_typewriterService != null)
        {
            _typewriterService.AnimationStateChanged -= OnAnimationStateChanged;
            _typewriterService.StopAnimation();
        }

        // Cancel and cleanup glow animation
        _glowAnimationCancellation?.Cancel();
        _glowAnimationCancellation?.Dispose();
        _glowAnimationCancellation = null;
    }

    private async Task SetupLanguageTranslations()
    {
        await LocalizationService.InitAsync();
    }

    private void AdaptLayout()
    {
        _layoutManager.AdaptLayout(
            null,
            NarrativeLabel,
            CharacterImage,
            CharacterNameLabel,
            null,
            MainLayoutGrid);
    }

    private void UpdateProgressCard()
    {
        var completedIngredients = _flowController.GetCollectedIngredientsDisplay();
        var ingredientCount = _flowController.GetCollectedIngredientsCount();

        IngredientsProgressLabel.Text = string.Format(LocalizationService.Translate("free_math_progress_label"), ingredientCount);

        var progressPercentage = (double)ingredientCount / 7.0; // Back to 7 ingredients

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
                var screenWidth = displayInfo.Width / displayInfo.Density;
                var estimatedContainerWidth = screenWidth - 50;

                var containerWidth = Math.Max(280, Math.Min(estimatedContainerWidth, 400));

                var progressWidth = containerWidth * progressPercentage;

                if (ingredientCount > 0 && progressWidth < 20)
                {
                    progressWidth = 20;
                }
                else if (ingredientCount >= 7) // Back to 7 ingredients
                {
                    progressWidth = containerWidth;
                }

                ProgressBarFill.WidthRequest = progressWidth;
            }
            catch (Exception ex)
            {
                var fallbackWidth = 280 * progressPercentage;
                if (ingredientCount >= 7) fallbackWidth = 280; // Back to 7 ingredients
                ProgressBarFill.WidthRequest = fallbackWidth;
            }
        });

        if (ingredientCount == 0)
        {
            IngredientsDetailsLabel.Text = LocalizationService.Translate("free_math_progress_start");
        }
        else if (ingredientCount >= 7) // Back to 7 ingredients
        {
            IngredientsDetailsLabel.Text = LocalizationService.Translate("free_math_progress_complete");
        }
        else
        {
            IngredientsDetailsLabel.Text = $"{string.Join(", ", completedIngredients)}";
        }

        UpdateProgressButtonState();
    }

    private void UpdateProgressButtonState()
    {
        MapNotificationBadge.IsVisible = _hasNewProgressUpdate;

        if (_hasNewProgressUpdate)
        {
            ProgressMapButton.BackgroundColor = Color.FromArgb("#FF6B35");
            StartMapBadgeAnimation();
        }
        else
        {
            ProgressMapButton.BackgroundColor = Color.FromArgb("#4A90E2");
        }
    }

    private async void StartMapBadgeAnimation()
    {
        if (!MapNotificationBadge.IsVisible || !_hasNewProgressUpdate) return;

        try
        {
            while (MapNotificationBadge.IsVisible && _hasNewProgressUpdate)
            {
                await MapNotificationBadge.ScaleTo(1.3, 400);
                await MapNotificationBadge.ScaleTo(1.0, 400);
                await Task.Delay(800);
            }
        }
        catch
        {
            // Silent fail
        }
    }

    private async Task UpdateCurrentStep()
    {
        if (_isUpdatingStep)
        {
            return;
        }

        _isUpdatingStep = true;

        try
        {
            _typewriterService?.StopAnimation();

            var currentStep = _flowController.GetCurrentStep();

            _fullNarrativeText = currentStep.NarrativeText;

            UpdateCharacterDisplay(currentStep);

            // Play animation as normal
            _isCharacterInteractive = false;
            UpdateCharacterInteractionState();

            // Remove Task.Delay - let the typewriter service handle its own timing
            await _typewriterService.StartTypewriterAnimation(NarrativeLabel, _fullNarrativeText);
        }
        catch (Exception ex)
        {
            NarrativeLabel.Text = _fullNarrativeText;
            _isCharacterInteractive = true;
            UpdateCharacterInteractionState();
        }
        finally
        {
            _isUpdatingStep = false;
        }
    }

    private void OnStopAnimationClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_fullNarrativeText) &&
            NarrativeLabel.Text == _fullNarrativeText)
        {
            _isCharacterInteractive = true;
            // Mark as completed

            UpdateCharacterInteractionState();
            StartGlowAnimation();
        }
    }

    // Add a flag to track animation completion state persistently
    private void OnAnimationStateChanged(object sender, bool isCompleted)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _isCharacterInteractive = isCompleted; // Store completion state persistently

            UpdateCharacterInteractionState();

            if (isCompleted)
            {
                StartGlowAnimation();
            }
        });
    }

    private void UpdateCharacterInteractionState()
    {
        if (_isCharacterInteractive)
        {
            // Interactive state: blue color
            InteractionCircle.Stroke = Color.FromArgb("#1E90FF");
            InteractionCircle.Opacity = 0.8;
            CharacterImage.Opacity = 1.0;
        }
        else
        {
            // Non-interactive state: gray color
            InteractionCircle.Stroke = Colors.Gray;
            InteractionCircle.Opacity = 0.6;
            CharacterImage.Opacity = 0.7;
        }
    }

    private async void StartGlowAnimation()
    {
        if (!_isCharacterInteractive) return;

        // Cancel any previous glow animation
        _glowAnimationCancellation?.Cancel();
        _glowAnimationCancellation = new CancellationTokenSource();

        try
        {
            while (_isCharacterInteractive && !_typewriterService.IsAnimating)
            {
                if (_glowAnimationCancellation.Token.IsCancellationRequested) return;

                // Blue glow animation - alternate between two blue shades
                InteractionCircle.Stroke = Color.FromArgb("#1E90FF"); // Standard blue
                await InteractionCircle.FadeTo(1.0, 1000);

                if (_glowAnimationCancellation.Token.IsCancellationRequested) return;

                await InteractionCircle.FadeTo(0.8, 1000);

                if (_glowAnimationCancellation.Token.IsCancellationRequested) return;

                await Task.Delay(500, _glowAnimationCancellation.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Animation was cancelled, this is normal
        }
        catch (Exception)
        {
            // Silently ignore any other issues while animating
        }
    }

    private async Task HandleProblemSolved()
    {
        if (_isUpdatingStep) return;

        var currentIngredient = PreferencesManager.GetCurrentIngredient();
        _flowController.CollectIngredient(currentIngredient);

        var updatedCount = _flowController.GetCollectedIngredientsCount();
        var collectedIngredients = _flowController.GetCollectedIngredients();

        _hasNewProgressUpdate = true;

        UpdateProgressCard();

        //CharacterImage.Source = "puf_succeded.png";
        //CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");

        _flowController.AdvanceToNextStep();

        // Remove Task.Delay - show success feedback immediately then proceed
        await UpdateCurrentStep();
    }

    private async void OnCharacterImageTapped(object sender, EventArgs e)
    {
        if (_typewriterService.IsAnimating)
        {
            return;
        }

        // Check both interactive state and animation completion for more reliability
        if (_isCharacterInteractive)
        {
            var currentStep = _flowController.GetCurrentStep();

            if (currentStep.Problem != null)
            {
                await Shell.Current.GoToAsync("FreeMathProblemPage", new Dictionary<string, object>
                {
                    ["problem"] = currentStep.Problem
                });
            }
            else if (currentStep.NextStepIndex.HasValue)
            {
                _flowController.AdvanceToNextStep();
                await UpdateCurrentStep();
            }
            else
            {
                // This is the end of the wizard rescue quest - let user decide when to complete!
                CharacterImage.Source = "puf_succeded.png";

                // Show completion popup and wait for user decision
                bool userWantsToComplete = await _tutorialPopupManager.ShowGameCompletionPopup();

                if (userWantsToComplete)
                {
                    // User decided to complete the game - trigger the completion
                    await _mainGameFlowController.CompleteGame();

                    // Navigate to main page
                    await Shell.Current.GoToAsync("//MainPage");
                }
                // If user doesn't want to complete (shouldn't happen with current UI, but just in case)
                // the popup will just close and they can tap again later
            }
        }
    }

    private async void OnProgressMapClicked(object sender, EventArgs e)
    {
        _hasNewProgressUpdate = false;

        UpdateProgressCard();
        UpdateProgressMapImage();

        ProgressImageOverlay.IsVisible = true;
        ProgressImageOverlay.Opacity = 0;
        await ProgressImageOverlay.FadeTo(1, 300);
    }

    private void UpdateProgressMapImage()
    {
        var imageName = _flowController.GetProgressMapImageName();
        ProgressMapImage.Source = imageName;
    }

    private async void OnCloseProgressClicked(object sender, EventArgs e)
    {
        await ProgressImageOverlay.FadeTo(0, 300);
        ProgressImageOverlay.IsVisible = false;
    }

    private async void OnProgressOverlayTapped(object sender, EventArgs e)
    {
        await ProgressImageOverlay.FadeTo(0, 300);
        ProgressImageOverlay.IsVisible = false;
    }

    private async void OnBackToMainClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnResetStoryClicked(object sender, EventArgs e)
    {
        var title = LocalizationService.Translate("free_math_reset_title");
        var message = LocalizationService.Translate("free_math_reset_message");
        var yes = LocalizationService.Translate("free_math_reset_yes");
        var no = LocalizationService.Translate("free_math_reset_no");

        bool confirm = await DisplayAlert(title, message, yes, no);
        if (confirm)
        {
            _flowController.ResetProgress();
            _hasNewProgressUpdate = false;
            _tutorialShown = false;

            // Remove Task.Delay - flow control should be immediate
            _flowController.LoadProgress();

            UpdateProgressCard();
            UpdateProgressMapImage();

            await UpdateCurrentStep();

            TutorialService.ResetCharacterInteractionTutorial();
        }
    }

    private void UpdateCharacterDisplay(FreeMathStep currentStep)
    {
        // First priority: use the explicit CharacterImageUrl if available
        if (!string.IsNullOrEmpty(currentStep.CharacterImageUrl))
        {
            CharacterImage.Source = currentStep.CharacterImageUrl;

            // Set character name based on the image
            var characterName = GetCharacterNameFromImageUrl(currentStep.CharacterImageUrl);
            CharacterNameLabel.Text = characterName;
        }
        // Fallback: use problem ingredient if available (for backward compatibility)
        else if (currentStep.Problem != null)
        {
            var ingredient = currentStep.Problem.Ingredient;
            switch (ingredient)
            {
                case IngredientType.CarrotDust:
                    CharacterImage.Source = "bunny.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_bunny");
                    break;
                case IngredientType.MouseTail:
                    CharacterImage.Source = "cat.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_cat");
                    break;
                case IngredientType.FishBones:
                    CharacterImage.Source = "dog.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_dog");
                    break;
                case IngredientType.MilletSeeds:
                    CharacterImage.Source = "parrot.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_parrot");
                    break;
                case IngredientType.BambooElixir:
                    CharacterImage.Source = "panda.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_panda");
                    break;
                case IngredientType.MagicWand:
                    CharacterImage.Source = "koala.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_koala");
                    break;
                case IngredientType.WizardHat:
                    CharacterImage.Source = "owl.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("animal_names_owl");
                    break;
                default:
                    CharacterImage.Source = "puf_wondering.png";
                    CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");
                    break;
            }
        }
        else
        {
            // Default fallback
            CharacterImage.Source = "puf_wondering.png";
            CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");
        }
    }

    private string GetCharacterNameFromImageUrl(string imageUrl)
    {
        return imageUrl switch
        {
            "bunny.png" => LocalizationService.Translate("animal_names_bunny"),
            "cat.png" => LocalizationService.Translate("animal_names_cat"),
            "dog.png" => LocalizationService.Translate("animal_names_dog"),
            "parrot.png" => LocalizationService.Translate("animal_names_parrot"),
            "panda.png" => LocalizationService.Translate("animal_names_panda"),
            "koala.png" => LocalizationService.Translate("animal_names_koala"),
            "owl.png" => LocalizationService.Translate("animal_names_owl"),
            "puf_wondering.png" or "puf_succeded.png" or "pufpuf.png" => LocalizationService.Translate("character_puf_puf"),
            _ => LocalizationService.Translate("character_puf_puf")
        };
    }
}