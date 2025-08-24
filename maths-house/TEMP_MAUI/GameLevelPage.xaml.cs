using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;
using GCMS.MathHouse.UI.Common;
using GCMS.MathHouse.UI.GameLevel;

namespace GCMS.MathHouse;

public partial class GameLevelPage : ContentPage
{
    private readonly GameLevelUIController _uiController;
    private readonly GameLevelLayoutManager _layoutManager;
    private readonly GameLevelFlowController _flowController;
    private readonly AudioManager _audioManager;
    private readonly AnimationManager _animationManager;

    public GameLevelPage(GameLevelService gameLevelService, ResponsiveLayoutService layoutService)
    {
        InitializeComponent();
        _uiController = new GameLevelUIController(gameLevelService);
        _layoutManager = new GameLevelLayoutManager(layoutService);
        _flowController = new GameLevelFlowController(gameLevelService, _uiController);
        _audioManager = new AudioManager();
        _animationManager = new AnimationManager();
        InitializeUIElements();
        SetupKeyboardEvents();
    }

    private void InitializeUIElements()
    {
        _uiController.InitializeUIElements(
            GameMessageLabel, FeedbackLabel, ScoreSummaryLabel,
            ExerciseLabel, AnswerDisplayLabel, QuestionResultsLabel,
            StartButton, RetryButton, CongratulationsButton,
            ExerciseFrame, AnswerFrame, NumericKeyboardFrame,
            QuestionStatusFrame, NextButton,
            QuestionStatusButtonsStack, QuestionResultsStack,
            PufPufHelperImage, StartDoorImage, EndDoorImage, StartDoorImageFrame);
    }

    private void SetupKeyboardEvents()
    {
        KeyboardControl.NumberClicked += (sender, digit) => _flowController.OnNumberClicked(digit);
        KeyboardControl.DeleteClicked += (sender, e) => _flowController.OnDeleteClicked();
        KeyboardControl.OkClicked += (sender, e) => _flowController.OnOkClicked();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _uiController.ResetGameUI();
        await SetupLanguageTranslations();
        _layoutManager.AdaptLayout(GameMessageLabel, FeedbackLabel, ScoreSummaryLabel);
        await _audioManager.PlayMainMusic(audioElement);
        await _animationManager.AnimateStartDoorGlow(StartDoorImage);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animationManager.StopGlow();
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

    private async Task SetupLanguageTranslations()
    {
        await LocalizationService.InitAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _uiController.SetupLanguageTranslations();
        });
    }

    private void OnStartGameClicked(object sender, EventArgs e)
    {
        _flowController.StartGame();
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        _flowController.OnOkClicked();
    }

    private void OnRetryClicked(object sender, EventArgs e)
    {
        _flowController.OnRetryClicked();
    }

    private void OnCongratulationsClicked(object sender, EventArgs e)
    {
        _flowController.OnCongratulationsClicked();
    }
}