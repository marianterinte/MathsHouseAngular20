import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TypewriterService {
  private timer: any = null;

  start(text: string, onUpdate: (current: string) => void, speedMs = 30): { stop: () => void } {
    this.stop();
    let i = 0;
    onUpdate('');
    this.timer = setInterval(() => {
      if (i >= text.length) {
        this.stop();
        return;
      }
      i++;
      onUpdate(text.slice(0, i));
    }, speedMs);
    return { stop: () => this.stop() };
  }

  stop() {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }
}
