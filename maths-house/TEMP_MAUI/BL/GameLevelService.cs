using System.Text;

namespace GCMS.MathHouse.BL
{
    public class GameLevelService
    {
        public delegate void GameCompletedCallback(bool success);

        private GameCompletedCallback _callback;
        private MathOperationType _currentOperation;
        private int _currentQuestionIndex;
        private int _correctAnswers;
        private int _totalQuestions;
        private readonly Random _rand = new();
        private readonly List<QuestionResult> _results = new();
        private readonly HashSet<string> _askedQuestionTexts = new();
        private QuestionResult _currentQuestion;
        
        public GameLevelService()
        {
            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _totalQuestions = 0;
        }

        public void Initialize(MathOperationType operationType, int repeatCount, GameCompletedCallback callback)
        {
            _callback = callback;
            _currentOperation = operationType;
            _totalQuestions = repeatCount;

            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _results.Clear();
            _askedQuestionTexts.Clear();
        }

        public void StartGame()
        {
            Reset();
            GenerateQuestion();
        }

        public bool IsOver()
        {
            return _currentQuestionIndex >= _totalQuestions;
        }

        public void Reset()
        {
            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _results.Clear();
            _askedQuestionTexts.Clear();
            _currentQuestion = null;
        }

        public string GenerateQuestion()
        {
            int number1 = 0, number2 = 0, number3 = 0;
            string questionText = string.Empty;
            int correctAnswer = 0;

            bool isValidQuestion;

            do
            {
                // resetăm argumentele
                number1 = number2 = number3 = 0;
                questionText = string.Empty;
                correctAnswer = 0;
                isValidQuestion = true;

                switch (_currentOperation)
                {
                    case MathOperationType.AddTwoNumbersLessThan10:
                        number1 = _rand.Next(1, 6);
                        number2 = _rand.Next(1, 6);
                        correctAnswer = number1 + number2;
                        break;

                    case MathOperationType.AddTwoNumbersLessThan20:
                        number1 = _rand.Next(1, 11);
                        number2 = _rand.Next(1, 11);
                        correctAnswer = number1 + number2;
                        break;

                    case MathOperationType.AddThreeNumbersLessThan10:
                        number1 = _rand.Next(1, 4);
                        number2 = _rand.Next(1, 4);
                        number3 = _rand.Next(1, 10 - (number1 + number2));
                        correctAnswer = number1 + number2 + number3;
                        break;

                    case MathOperationType.SubtractThreeNumbersLessThan10:
                        number1 = _rand.Next(0, 10);
                        number2 = _rand.Next(0, number1 + 1);
                        number3 = _rand.Next(0, number1 - number2 + 1);
                        correctAnswer = number1 - number2 - number3;
                        break;

                    case MathOperationType.AddAndSubstract3NumbersLessThan10:
                        number1 = _rand.Next(1, 10);
                        number2 = _rand.Next(1, 10);
                        number3 = _rand.Next(1, 10);
                        correctAnswer = number1 + number2 - number3;
                        break;

                    case MathOperationType.AddAndSubstract2NumbersLessThan20:
                        number1 = _rand.Next(1, 20);
                        number2 = _rand.Next(1, 20);
                        number3 = _rand.Next(1, 20);
                        correctAnswer = number1 + number2 - number3;
                        break;

                    default:
                        number1 = _rand.Next(1, 6);
                        number2 = _rand.Next(1, 6);
                        correctAnswer = number1 + number2;
                        break;
                }

                // construim textul întrebării (după calcul)
                questionText = _currentOperation switch
                {
                    MathOperationType.AddThreeNumbersLessThan10 => $"{number1} + {number2} + {number3} = ",
                    MathOperationType.SubtractThreeNumbersLessThan10 => $"{number1} - {number2} - {number3} = ",
                    MathOperationType.AddAndSubstract3NumbersLessThan10 => $"{number1} + {number2} - {number3} = ",
                    MathOperationType.AddAndSubstract2NumbersLessThan20 => $"{number1} + {number2} - {number3} = ",
                    _ => $"{number1} + {number2} = "
                };

                // validăm: fără duplicate și fără răspunsuri negative
                if (correctAnswer < 0 || _askedQuestionTexts.Contains(questionText))
                {
                    isValidQuestion = false;
                }

            } while (!isValidQuestion && _askedQuestionTexts.Count < 100);

            // Înregistrăm întrebarea și răspunsul
            _askedQuestionTexts.Add(questionText);
            _currentQuestion = new QuestionResult
            {
                QuestionText = questionText,
                CorrectAnswer = correctAnswer
            };

            return questionText;
        }


        public void CheckAnswer(int answer)
        {
            _currentQuestion.UserAnswer = answer;
            _results.Add(_currentQuestion);

            if (_currentQuestion.IsCorrect)
            {
                _correctAnswers++;
            }

            _currentQuestionIndex++;

            if (!IsOver())
            {
                GenerateQuestion();
            }
            //else
            //{
            //    bool success = _correctAnswers == _totalQuestions;
            //    _callback?.Invoke(success);
            //}
        }
        public void CompleteGame()
        {
            _callback?.Invoke(_correctAnswers == _totalQuestions);
        }

        public void CompleteGameFAke()
        {
            _callback?.Invoke(true);
        }

        public List<QuestionResult> GetQuestionResults()
        {
            return _results;
        }

        public int GetCorrectAnswers() => _correctAnswers;

        public int GetTotalQuestions() => _totalQuestions;

        public string GetCipherCode()
        {
            var finalCipherCode = new StringBuilder();
            foreach (var result in _results)
            {
                if (result.IsCorrect)
                {
                    finalCipherCode.Append($"✅ " + result.UserAnswer);
                }
                else
                {
                    finalCipherCode.Append($"🔐 " + result.UserAnswer);
                }
            }

            return finalCipherCode.ToString();
        }

        public bool CheckAnswerForCurrentQuestion(int userAnswer)
        {
            _currentQuestion.UserAnswer = userAnswer;
            _results.Add(_currentQuestion);

            if (_currentQuestion.IsCorrect)
            {
                _correctAnswers++;
            }

            _currentQuestionIndex++;

            return _currentQuestion.IsCorrect;
        }

        public string GenerateQuestionForIndex(int questionIndex)
        {
            int number1 = 0, number2 = 0, number3 = 0;
            string questionText = string.Empty;
            int correctAnswer = 0;

            bool isValidQuestion;

            do
            {
                number1 = number2 = number3 = 0;
                questionText = string.Empty;
                correctAnswer = 0;
                isValidQuestion = true;

                switch (_currentOperation)
                {
                    case MathOperationType.AddTwoNumbersLessThan10:
                        number1 = _rand.Next(1, 6);
                        number2 = _rand.Next(1, 6);
                        correctAnswer = number1 + number2;
                        break;

                    case MathOperationType.AddTwoNumbersLessThan20:
                        number1 = _rand.Next(1, 11);
                        number2 = _rand.Next(1, 11);
                        correctAnswer = number1 + number2;
                        break;

                    case MathOperationType.AddThreeNumbersLessThan10:
                        number1 = _rand.Next(1, 4);
                        number2 = _rand.Next(1, 4);
                        number3 = _rand.Next(1, 10 - (number1 + number2));
                        correctAnswer = number1 + number2 + number3;
                        break;

                    case MathOperationType.SubtractThreeNumbersLessThan10:
                        number1 = _rand.Next(0, 10);
                        number2 = _rand.Next(0, number1 + 1);
                        number3 = _rand.Next(0, number1 - number2 + 1);
                        correctAnswer = number1 - number2 - number3;
                        break;

                    case MathOperationType.AddAndSubstract3NumbersLessThan10:
                        number1 = _rand.Next(1, 10);
                        number2 = _rand.Next(1, 10);
                        number3 = _rand.Next(1, 10);
                        correctAnswer = number1 + number2 - number3;
                        break;

                    case MathOperationType.AddAndSubstract2NumbersLessThan20:
                        number1 = _rand.Next(1, 20);
                        number2 = _rand.Next(1, 20);
                        number3 = _rand.Next(1, 20);
                        correctAnswer = number1 + number2 - number3;
                        break;

                    default:
                        number1 = _rand.Next(1, 6);
                        number2 = _rand.Next(1, 6);
                        correctAnswer = number1 + number2;
                        break;
                }

                questionText = _currentOperation switch
                {
                    MathOperationType.AddThreeNumbersLessThan10 => $"{number1} + {number2} + {number3} = ",
                    MathOperationType.SubtractThreeNumbersLessThan10 => $"{number1} - {number2} - {number3} = ",
                    MathOperationType.AddAndSubstract3NumbersLessThan10 => $"{number1} + {number2} - {number3} = ",
                    MathOperationType.AddAndSubstract2NumbersLessThan20 => $"{number1} + {number2} - {number3} = ",
                    _ => $"{number1} + {number2} = "
                };

                if (correctAnswer < 0 || _askedQuestionTexts.Contains(questionText))
                {
                    isValidQuestion = false;
                }

            } while (!isValidQuestion && _askedQuestionTexts.Count < 100);

            _askedQuestionTexts.Add(questionText);
            _currentQuestion = new QuestionResult
            {
                QuestionText = questionText,
                CorrectAnswer = correctAnswer
            };

            return questionText;
        }

        public bool AreAllQuestionsCorrect()
        {
            return _results.Count == _totalQuestions && _results.All(r => r.IsCorrect);
        }

        public int GetFirstIncorrectQuestionIndex()
        {
            for (int i = 0; i < _results.Count; i++)
            {
                if (!_results[i].IsCorrect)
                    return i;
            }
            return -1;
        }
    }
}
