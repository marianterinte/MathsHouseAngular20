export interface QuestionResult {
  questionText: string;
  correctAnswer: number;
  userAnswer?: number;
  get isCorrect(): boolean;
}

export function makeResult(questionText: string, correctAnswer: number): QuestionResult {
  return {
    questionText,
    correctAnswer,
    get isCorrect() { return this.userAnswer === this.correctAnswer; }
  } as QuestionResult;
}