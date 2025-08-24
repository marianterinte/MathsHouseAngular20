import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ResponsiveService {
  // current width/height for quick decisions
  width = signal<number>(typeof window !== 'undefined' ? window.innerWidth : 0);
  height = signal<number>(typeof window !== 'undefined' ? window.innerHeight : 0);

  update() {
    if (typeof window === 'undefined') return;
    this.width.set(window.innerWidth);
    this.height.set(window.innerHeight);
  }
}
