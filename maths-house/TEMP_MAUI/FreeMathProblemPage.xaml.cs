using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;

namespace GCMS.MathHouse;

[QueryProperty(nameof(Problem), "problem")]
public partial class FreeMathProblemPage : ContentPage
{
    private MathProblem _problem;
    private string _currentAnswer = "";
    private readonly AudioManager _audioManager;
    private readonly TutorialPopupManager _tutorialPopupManager;
    private readonly ResponsiveLayoutService _layoutService;
    private bool _tutorialShown = false; // Track if tutorial has been shown this session
    private int _attemptsLeft = 3; // Track attempts remaining
    private readonly Label[] _hearts; // Array to hold heart references
    
    public MathProblem Problem
    {
        get => _problem;
        set
        {
            _problem = value;
            OnPropertyChanged();
            if (_problem != null)
            {
                UpdateUI();
            }
        }
    }

    public FreeMathProblemPage(ResponsiveLayoutService layoutService)
    {
        InitializeComponent();
        _audioManager = new AudioManager();
        _tutorialPopupManager = new TutorialPopupManager();
        _layoutService = layoutService;
        
        // Initialize hearts array
        _hearts = new Label[] { Heart1, Heart2, Heart3 };
        
        SetupKeyboardEvents();
        SetupCharacterDisplay();
        InitializeLayout();
    }

    private async void InitializeLayout()
    {
        await _layoutService.InitializeAsync();
        AdaptLayout();
    }

    private void AdaptLayout()
    {
        // Apply responsive font sizes
        ProblemLabel.FontSize = _layoutService.GetFontSize("FreeMathNarrative");
        EncouragingLabel.FontSize = _layoutService.GetFontSize("FreeMathNarrative");
        CharacterNameLabel.FontSize = _layoutService.GetFontSize("CharacterName");
        EquationLabel.FontSize = _layoutService.GetFontSize("FreeMathNarrative");
        AnswerDisplayLabel.FontSize = _layoutService.GetFontSize("FreeMathNarrative");
        
        // Responsive spacing based on screen size
        var isLargeScreen = DeviceDisplay.MainDisplayInfo.Width > 800;
        var rowSpacing = isLargeScreen ? 15 : 8;
        
        if (Content is ScrollView scrollView && scrollView.Content is Grid grid)
        {
            grid.RowSpacing = rowSpacing;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Play the same music as GameLevelPage
        await _audioManager.PlayMainMusic(audioElement);
        
        // Show tutorial if this is the first time and haven't shown yet this session
        if (!_tutorialShown && TutorialService.ShouldShowCharacterInteractionTutorial())
        {
            _tutorialShown = true; // Prevent multiple shows in same session
            // Remove Task.Delay - show tutorial immediately after page is ready
            await _tutorialPopupManager.ShowCharacterInteractionTutorial();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
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

    private void SetupKeyboardEvents()
    {
        KeyboardControl.NumberClicked += OnNumberClicked;
        KeyboardControl.DeleteClicked += OnDeleteClicked;
        KeyboardControl.OkClicked += OnOkClicked;
    }

    private void SetupCharacterDisplay()
    {
        // Initialize character interaction circle - make it interactive
        InteractionCircle.Stroke = Color.FromArgb("#1E90FF"); // Dodger Blue
        InteractionCircle.Opacity = 0.8;
        CharacterImage.Opacity = 1.0;
    }

    private void UpdateUI()
    {
        if (Problem == null) return;

        ProblemLabel.Text = Problem.Statement;
        
        // Reset attempts and hearts
        _attemptsLeft = 3;
        UpdateHeartsDisplay();
        
        // Update character based on the problem's ingredient
        UpdateCharacterDisplay();
        
        // Extract equation part for display
        var statement = Problem.Statement;
        if (statement.Contains("?"))
        {
            // Try to extract equation from statement
            var parts = statement.Split(' ');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i].Contains("=") || (i < parts.Length - 1 && parts[i + 1] == "="))
                {
                    EquationLabel.Text = $"{Problem.Statement.Split('?')[0]}? =";
                    break;
                }
            }
        }
        else
        {
            EquationLabel.Text = LocalizationService.Translate("problem_page_answer_equals");
        }
        
        _currentAnswer = "";
        UpdateAnswerDisplay();
        FeedbackLabel.IsVisible = false;
    }

    private void UpdateHeartsDisplay()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            if (i < _attemptsLeft)
            {
                _hearts[i].Text = "❤️";
                _hearts[i].Opacity = 1.0;
            }
            else
            {
                _hearts[i].Text = "💔";
                _hearts[i].Opacity = 0.5;
            }
        }
    }

    private async Task AnimateHeartLoss()
    {
        var heartIndex = 3 - _attemptsLeft - 1; // Convert to 0-based index
        if (heartIndex >= 0 && heartIndex < _hearts.Length)
        {
            var heart = _hearts[heartIndex];
            
            // Scale animation to show heart breaking
            await heart.ScaleTo(1.5, 200);
            heart.Text = "💔";
            heart.Opacity = 0.5;
            await heart.ScaleTo(1.0, 200);
        }
    }

    private void UpdateCharacterDisplay()
    {
        if (Problem == null) return;

        // Show the animal that's asking for the ingredient
        var ingredient = Problem.Ingredient;
        switch (ingredient)
        {
            case IngredientType.CarrotDust:
                CharacterImage.Source = "bunny.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_bunny");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_carrot_dust_help");
                break;
            case IngredientType.MouseTail:
                CharacterImage.Source = "cat.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_cat");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_mouse_tail_help");
                break;
            case IngredientType.FishBones:
                CharacterImage.Source = "dog.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_dog");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_fish_bones_help");
                break;
            case IngredientType.MilletSeeds:
                CharacterImage.Source = "parrot.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_parrot");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_millet_seeds_help");
                break;
            case IngredientType.BambooElixir:
                CharacterImage.Source = "panda.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_panda");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_bamboo_elixir_help");
                break;
            case IngredientType.MagicWand:
                CharacterImage.Source = "koala.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_koala");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_magic_wand_help");
                break;
            case IngredientType.WizardHat:
                CharacterImage.Source = "owl.png";
                CharacterNameLabel.Text = LocalizationService.Translate("animal_names_owl");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_wizard_hat_help");
                break;
            default:
                CharacterImage.Source = "puf_wondering.png";
                CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_default_help");
                break;
        }
    }

    private async void OnCharacterImageTapped(object sender, EventArgs e)
    {
        // Provide a hint or encouragement when character is tapped
        var title = string.Format(LocalizationService.Translate("problem_page_hint_dialog_title"), CharacterNameLabel.Text);
        var message = LocalizationService.Translate("problem_page_hint_dialog_message");
        var button = LocalizationService.Translate("problem_page_hint_dialog_button");
        await DisplayAlert(title, message, button);
    }

    private void OnNumberClicked(object sender, string digit)
    {
        if (_currentAnswer.Length < 3) // Limit to 3 digits
        {
            _currentAnswer += digit;
            UpdateAnswerDisplay();
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentAnswer.Length > 0)
        {
            _currentAnswer = _currentAnswer.Substring(0, _currentAnswer.Length - 1);
            UpdateAnswerDisplay();
        }
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        if (Problem == null || string.IsNullOrEmpty(_currentAnswer)) return;

        if (int.TryParse(_currentAnswer, out int userAnswer))
        {
            if (userAnswer == Problem.CorrectAnswer)
            {
                FeedbackLabel.Text = LocalizationService.Translate("problem_page_feedback_correct");
                FeedbackLabel.TextColor = Colors.Green;
                FeedbackLabel.IsVisible = true;

                // Show success character
                CharacterImage.Source = "puf_succeded.png";
                CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");
                EncouragingLabel.Text = LocalizationService.Translate("problem_page_feedback_excellent");

                // Use a timer to coordinate feedback display instead of Task.Delay
                var feedbackTimer = new Timer(async _ =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        // Stop music before navigating back
                        StopMusicSafely();
                        PreferencesManager.SetCurrentIngredient(Problem.Ingredient);

                        // Return to FreeMathPage with success result
                        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                        {
                            ["success"] = true,
                        });
                    });
                }, null, 1500, Timeout.Infinite);
            }
            else
            {
                // Wrong answer - reduce attempts
                _attemptsLeft--;
                await AnimateHeartLoss();
                
                if (_attemptsLeft > 0)
                {
                    // Still have attempts left - encourage to rethink
                    var encouragingMessages = new[]
                    {
                        LocalizationService.Translate("problem_page_feedback_wrong_try1"),
                        LocalizationService.Translate("problem_page_feedback_wrong_try2"),
                        LocalizationService.Translate("problem_page_feedback_wrong_try3")
                    };
                    
                    var messageIndex = 3 - _attemptsLeft - 1;
                    var message = messageIndex < encouragingMessages.Length 
                        ? encouragingMessages[messageIndex] 
                        : LocalizationService.Translate("problem_page_feedback_wrong_default");
                    
                    FeedbackLabel.Text = message;
                    FeedbackLabel.TextColor = Colors.Orange;
                    FeedbackLabel.IsVisible = true;
                    
                    // Clear the answer field so they can try again
                    _currentAnswer = "";
                    UpdateAnswerDisplay();
                }
                else
                {
                    // No attempts left - now show the correct answer
                    FeedbackLabel.Text = string.Format(LocalizationService.Translate("problem_page_feedback_no_attempts"), Problem.CorrectAnswer);
                    FeedbackLabel.TextColor = Colors.Red;
                    FeedbackLabel.IsVisible = true;
                    
                    // Use timer instead of Task.Delay for coordination
                    var correctAnswerTimer = new Timer(async _ =>
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            // Show success character anyway (we're being forgiving)
                            CharacterImage.Source = "puf_succeded.png";
                            CharacterNameLabel.Text = LocalizationService.Translate("character_puf_puf");
                            EncouragingLabel.Text = LocalizationService.Translate("problem_page_feedback_learning");
                            
                            // Set up final navigation timer
                            var navigationTimer = new Timer(async __ =>
                            {
                                await MainThread.InvokeOnMainThreadAsync(async () =>
                                {
                                    // Stop music before navigating back
                                    StopMusicSafely();

                                    // Return to FreeMathPage with success result (being forgiving)
                                    await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                                    {
                                        ["success"] = true,
                                        ["ingredient"] = Problem.Ingredient
                                    });
                                });
                            }, null, 1500, Timeout.Infinite);
                        });
                    }, null, 3000, Timeout.Infinite);
                }
            }
        }
        else
        {
            FeedbackLabel.Text = LocalizationService.Translate("problem_page_feedback_invalid_number");
            FeedbackLabel.TextColor = Colors.Orange;
            FeedbackLabel.IsVisible = true;
        }
    }

    private void UpdateAnswerDisplay()
    {
        AnswerDisplayLabel.Text = string.IsNullOrEmpty(_currentAnswer) ? "?" : _currentAnswer;
    }

    //private async void OnBackClicked(object sender, EventArgs e)
    //{
    //    StopMusicSafely();
    //    await Shell.Current.GoToAsync("..");
    //}
}