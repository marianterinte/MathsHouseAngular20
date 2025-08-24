using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse.UI.GameLevel;

public class GameLevelUIController
{
    private readonly GameLevelService _gameLevelService;
    private List<Button> _questionStatusButtons = new List<Button>();

    // UI Elements - vor fi injectate din GameLevelPage
    private Label _gameMessageLabel;
    private Label _feedbackLabel;
    private Label _scoreSummaryLabel;
    private Label _exerciseLabel;
    private Label _answerDisplayLabel;
    private Label _questionResultsLabel;
    private Button _startButton;
    private Button _retryButton;
    private Button _congratulationsButton;
    private Frame _exerciseFrame;
    private Border _answerFrame;
    private Frame _numericKeyboardFrame;
    private Frame _questionStatusFrame;
    private Button _nextButton;
    private StackLayout _questionStatusButtonsStack;
    private StackLayout _questionResultsStack;
    private Image _pufPufHelperImage;
    private Image _startDoorImage;
    private Image _endDoorImage;
    private Frame _startDoorImageFrame;

    public GameLevelUIController(GameLevelService gameLevelService)
    {
        _gameLevelService = gameLevelService;
    }

    public void InitializeUIElements(
        Label gameMessageLabel, Label feedbackLabel, Label scoreSummaryLabel,
        Label exerciseLabel, Label answerDisplayLabel, Label questionResultsLabel,
        Button startButton, Button retryButton, Button congratulationsButton,
        Frame exerciseFrame, Border answerFrame, Frame numericKeyboardFrame,
        Frame questionStatusFrame, Button nextButton,
        StackLayout questionStatusButtonsStack, StackLayout questionResultsStack,
        Image pufPufHelperImage, Image startDoorImage, Image endDoorImage, Frame startDoorImageFrame)
    {
        _gameMessageLabel = gameMessageLabel;
        _feedbackLabel = feedbackLabel;
        _scoreSummaryLabel = scoreSummaryLabel;
        _exerciseLabel = exerciseLabel;
        _answerDisplayLabel = answerDisplayLabel;
        _questionResultsLabel = questionResultsLabel;
        _startButton = startButton;
        _retryButton = retryButton;
        _congratulationsButton = congratulationsButton;
        _exerciseFrame = exerciseFrame;
        _answerFrame = answerFrame;
        _numericKeyboardFrame = numericKeyboardFrame;
        _questionStatusFrame = questionStatusFrame;
        _nextButton = nextButton;
        _questionStatusButtonsStack = questionStatusButtonsStack;
        _questionResultsStack = questionResultsStack;
        _pufPufHelperImage = pufPufHelperImage;
        _startDoorImage = startDoorImage;
        _endDoorImage = endDoorImage;
        _startDoorImageFrame = startDoorImageFrame;
    }

    public void ResetGameUI()
    {
        // Afișăm din nou butonul Start
        _startButton.IsVisible = true;

        // Ascundem restul
        _exerciseFrame.IsVisible = false;
        _answerFrame.IsVisible = false;
        _numericKeyboardFrame.IsVisible = false;
        _nextButton.IsVisible = false;
        _feedbackLabel.IsVisible = false;
        _questionResultsStack.IsVisible = false;
        _congratulationsButton.IsVisible = false;
        _retryButton.IsVisible = false;

        // Resetăm afișajul răspunsului
        _answerDisplayLabel.Text = "?";

        // Ascundem status buttons-urile
        _questionStatusFrame.IsVisible = false;
        ClearQuestionStatusButtons();

        // Resetăm imaginea de start
        _pufPufHelperImage.Source = "puf_at_door.png";

        // Adăugăm imaginea cu ușa închisă la start
        _startDoorImage.Source = "doorlocked.png";
        _startDoorImage.IsVisible = true;
        _startDoorImageFrame.IsVisible = true;
        _endDoorImage.IsVisible = false;
    }

    public void ShowGameStarted()
    {
        _startButton.IsVisible = false;
        _startDoorImageFrame.IsVisible = false;
        _startDoorImage.IsVisible = false;
        _exerciseFrame.IsVisible = true;
        _answerFrame.IsVisible = true;
        _numericKeyboardFrame.IsVisible = true;
        _feedbackLabel.IsVisible = true;
        _questionStatusFrame.IsVisible = true;

        CreateQuestionStatusButtons();
    }

    public void CreateQuestionStatusButtons()
    {
        ClearQuestionStatusButtons();
        _questionStatusButtonsStack.Children.Clear();

        int totalQuestions = _gameLevelService.GetTotalQuestions();

        for (int i = 0; i < totalQuestions; i++)
        {
            var button = new Button
            {
                Text = "⚪", // gri initial
                IsEnabled = false, // nu mai e clickable
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Gray,
                CornerRadius = 25,
                WidthRequest = 20,
                HeightRequest = 20,
                Margin = new Thickness(5)
            };

            _questionStatusButtons.Add(button);
            _questionStatusButtonsStack.Children.Add(button);
        }
    }

    public void ClearQuestionStatusButtons()
    {
        _questionStatusButtons.Clear();
    }

    public void UpdateQuestionStatusButton(int questionIndex, bool isCorrect)
    {
        if (questionIndex >= 0 && questionIndex < _questionStatusButtons.Count)
        {
            var button = _questionStatusButtons[questionIndex];
            if (isCorrect)
            {
                button.Text = "🔓"; // Unlocked icon
                button.BackgroundColor = Colors.LightGreen;
                button.TextColor = Colors.DarkGreen;
            }
            else
            {
                button.Text = "🔒"; // Keep locked icon
                button.BackgroundColor = Colors.LightCoral;
                button.TextColor = Colors.DarkRed;
            }
        }
    }

    public void UpdateQuestion(string questionText)
    {
        _exerciseLabel.Text = questionText;
        _feedbackLabel.Text = "";
        _answerDisplayLabel.Text = "?";
    }

    public void ShowFeedback(string message, bool isCorrect)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.TextColor = isCorrect ? Colors.Green : Colors.Red;
    }

    public void ShowInputError(string message)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.TextColor = Colors.Red;
    }

    public void ShowIncompleteMessage(string message)
    {
        // Ascundem tastatura + exercise frame
        _exerciseFrame.IsVisible = false;
        _answerFrame.IsVisible = false;
        _numericKeyboardFrame.IsVisible = false;

        // Afișăm mesaj clar pentru utilizator
        _feedbackLabel.Text = message;
        _feedbackLabel.TextColor = Colors.Orange;
        _feedbackLabel.IsVisible = true;
    }

    public void EndGame(bool success)
    {
        if (success)
        {
            _gameMessageLabel.Text = LocalizationService.Translate("game_congrats");
            _congratulationsButton.IsVisible = true;
            _retryButton.IsVisible = false;
            _pufPufHelperImage.Source = "puf_succeded.png";

            // Afișăm imaginea cu ușa deschisă
            _endDoorImage.Source = "doorunlocked.png";
            _endDoorImage.IsVisible = true;
        }
        else
        {
            _gameMessageLabel.Text = LocalizationService.Translate("game_sorry");
            _retryButton.IsVisible = true;
            _congratulationsButton.IsVisible = false;
            _pufPufHelperImage.Source = "puf_at_door.png";
        }

        _questionResultsStack.IsVisible = true;
        _questionResultsLabel.Text = string.Join("\n", _gameLevelService.GetQuestionResults().Select(r =>
            $"{r.QuestionText.Replace("?", r.UserAnswer.ToString())} {(r.IsCorrect ? "✅" : "❌")}"));

        // Ascundem elementele de joc
        _exerciseFrame.IsVisible = false;
        _answerFrame.IsVisible = false;
        _numericKeyboardFrame.IsVisible = false;
        _nextButton.IsVisible = false;
        _questionStatusFrame.IsVisible = false;
    }

    public void UpdateAnswerDisplay(string answer)
    {
        _answerDisplayLabel.Text = string.IsNullOrEmpty(answer) ? "?" : answer;
    }

    public void SetupLanguageTranslations()
    {
        _startButton.Text = LocalizationService.Translate("game_start_button");
        _gameMessageLabel.Text = LocalizationService.Translate("game_start");
        _retryButton.Text = LocalizationService.Translate("game_retry");
        _congratulationsButton.Text = LocalizationService.Translate("game_continue");
    }
}
