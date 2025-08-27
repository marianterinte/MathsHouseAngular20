import { Injectable } from '@angular/core';
import { makeResult, QuestionResult } from '../models/question-result';

export type OperationType =
  | 'AddTwoNumbersLessThan10'
  | 'AddTwoNumbersLessThan20'
  | 'AddThreeNumbersLessThan10'
  | 'SubtractThreeNumbersLessThan10'
  | 'AddAndSubstract3NumbersLessThan10'
  | 'AddAndSubstract2NumbersLessThan20';

@Injectable({ providedIn: 'root' })
export class GameLevelService {
  private op: OperationType = 'AddTwoNumbersLessThan10';
  private total = 7;
  private idx = 0;
  private correct = 0;
  private current?: QuestionResult;
  private results: QuestionResult[] = [];
  private asked = new Set<string>();

  init(op: OperationType, total: number) {
    this.op = op; this.total = total;
    this.idx = 0; this.correct = 0; this.results = []; this.asked.clear(); this.current = undefined;
  }

  start() { this.idx = 0; this.correct = 0; this.results = []; this.asked.clear(); return this.nextQuestion(); }
  isOver() { return this.idx >= this.total; }
  get progress() { return { idx: this.idx, total: this.total, correct: this.correct, results: this.results.slice() }; }
  get currentQuestion(): QuestionResult | undefined { return this.current; }

  submit(answer: number): boolean {
    if (!this.current) return false;
    this.current.userAnswer = answer;
    this.results.push(this.current);
    if (this.current.isCorrect) this.correct++;
    this.idx++;
    if (!this.isOver()) this.nextQuestion();
    return this.current.isCorrect;
  }

  private nextQuestion(): QuestionResult {
    // simple random like MAUI service
    const rng = (min: number, max: number) => Math.floor(Math.random() * (max - min)) + min;
    let a=0,b=0,c=0, text='', correct=0;
    let ok=false, guard=0;
    while(!ok && guard++ < 100){
      a=b=c=0; text=''; correct=0; ok=true;
      switch(this.op){
        case 'AddTwoNumbersLessThan10': a=rng(1,6); b=rng(1,6); correct=a+b; break;
        case 'AddTwoNumbersLessThan20': a=rng(1,11); b=rng(1,11); correct=a+b; break;
        case 'AddThreeNumbersLessThan10': a=rng(1,4); b=rng(1,4); c=rng(1, Math.max(2,10-(a+b))); correct=a+b+c; break;
        case 'SubtractThreeNumbersLessThan10': a=rng(0,10); b=rng(0,a+1); c=rng(0, a-b+1); correct=a-b-c; break;
        case 'AddAndSubstract3NumbersLessThan10': a=rng(1,10); b=rng(1,10); c=rng(1,10); correct=a+b-c; break;
        case 'AddAndSubstract2NumbersLessThan20': a=rng(1,20); b=rng(1,20); c=rng(1,20); correct=a+b-c; break;
      }
      text = (this.op==='AddThreeNumbersLessThan10'||this.op==='SubtractThreeNumbersLessThan10'||this.op==='AddAndSubstract3NumbersLessThan10' || this.op==='AddAndSubstract2NumbersLessThan20')
        ? `${a} + ${b} - ${c} = `
        : `${a} + ${b} = `;
      if (correct < 0 || this.asked.has(text)) ok = false;
    }
    this.asked.add(text);
    this.current = makeResult(text, correct);
    return this.current;
  }
}