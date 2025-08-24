

using GCMS.MathHouse.BL;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse.UI.GameLevel;

public class GameLevelFlowController
{
    private readonly GameLevelService _gameLevelService;
    private readonly GameLevelUIController _uiController;
    private string _currentAnswer = "";
    private int _currentQuestionIndex = 0;

    public GameLevelFlowController(GameLevelService gameLevelService, GameLevelUIController uiController)
    {
        _gameLevelService = gameLevelService;
        _uiController = uiController;
    }

    public void StartGame()
    {
        _gameLevelService.StartGame();
        _uiController.ShowGameStarted();
        _currentQuestionIndex = 0;
        GenerateNextQuestion();
    }

    public void GenerateNextQuestion()
    {
        var questionText = _gameLevelService.GenerateQuestion();
        _uiController.UpdateQuestion(questionText);
        _currentAnswer = "";
    }

    public void RetrySpecificQuestion(int questionIndex)
    {
        // Generăm o nouă întrebare pentru acest index
        var newQuestion = _gameLevelService.GenerateQuestionForIndex(questionIndex);
        _uiController.UpdateQuestion(newQuestion);
        _currentAnswer = "";
    }

    public void OnNumberClicked(string digit)
    {
        // Limităm răspunsul la maxim 3 cifre pentru siguranță
        if (_currentAnswer.Length < 3)
        {
            if (_currentAnswer == "")
            {
                _currentAnswer = digit;
            }
            else
            {
                _currentAnswer += digit;
            }
            _uiController.UpdateAnswerDisplay(_currentAnswer);
        }
    }

    public void OnDeleteClicked()
    {
        if (_currentAnswer.Length > 0)
        {
            _currentAnswer = _currentAnswer.Substring(0, _currentAnswer.Length - 1);
            _uiController.UpdateAnswerDisplay(_currentAnswer);
        }
    }

    public void OnOkClicked()
    {
        if (string.IsNullOrEmpty(_currentAnswer))
        {
            var errorMessage = LocalizationService.Translate("game_input_error") ?? "Te rog introdu un răspuns!";
            _uiController.ShowInputError(errorMessage);
            return;
        }

        if (int.TryParse(_currentAnswer, out int userAnswer))
        {
            bool isCorrect = _gameLevelService.CheckAnswerForCurrentQuestion(userAnswer);

            // Actualizăm butonul de status pentru întrebarea curentă
            _uiController.UpdateQuestionStatusButton(_currentQuestionIndex, isCorrect);

            if (isCorrect)
            {
                var correctMessage = LocalizationService.Translate("game_correct") ?? "Corect!";
                _uiController.ShowFeedback(correctMessage, true);

                _currentQuestionIndex++;

                if (_currentQuestionIndex < _gameLevelService.GetTotalQuestions())
                {
                    GenerateNextQuestion();
                }
                else
                {
                    // Toate au fost corecte → END GAME
                    _uiController.EndGame(true);
                }
            }
            else
            {
                var incorrectMessage = LocalizationService.Translate("game_incorrect") ?? "Încorect! Încearcă din nou.";
                _uiController.ShowFeedback(incorrectMessage, false);

                // NU trecem la următoarea întrebare → regenerezi pentru același index
                RetrySpecificQuestion(_currentQuestionIndex);
            }
        }

        // Resetăm câmpul de răspuns pentru următoarea încercare
        _currentAnswer = "";
        _uiController.UpdateAnswerDisplay(_currentAnswer);
    }

    public void CheckGameCompletion()
    {
        // Verificăm dacă toate întrebările sunt rezolvate corect
        bool allQuestionsCorrect = _gameLevelService.AreAllQuestionsCorrect();

        if (allQuestionsCorrect)
        {
            _uiController.EndGame(true);
        }
        else
        {
            // Găsim prima întrebare greșită (doar ca să știm dacă sunt greșite)
            int firstIncorrectIndex = _gameLevelService.GetFirstIncorrectQuestionIndex();
            if (firstIncorrectIndex >= 0)
            {
                var incompleteMessage = LocalizationService.Translate("game_complete_all") ?? "Completează toate întrebările corect!";
                _uiController.ShowIncompleteMessage(incompleteMessage);
            }
        }
    }

    public void OnRetryClicked()
    {
        _gameLevelService.Reset();
        _uiController.ResetGameUI();
        _currentAnswer = "";
        _currentQuestionIndex = 0;
    }

    public void OnCongratulationsClicked()
    {
        _gameLevelService.CompleteGame();
    }
}