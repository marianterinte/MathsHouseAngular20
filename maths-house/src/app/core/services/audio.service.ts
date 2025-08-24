import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AudioService {
  // TODO: implement with Howler; placeholder API for now
  playMusic(channel: 'main' | 'level' | 'free'): void {}
  stopMusic(channel?: 'main' | 'level' | 'free'): void {}
  playSfx(name: string): void {}
}
