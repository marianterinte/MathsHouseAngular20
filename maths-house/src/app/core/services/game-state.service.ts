import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FloorId, FloorStatus } from '../models/enums';
import { GameProgress } from '../models/game-progress';

const STORAGE_KEY = 'GameProgress';

function defaultFloors(): Record<FloorId, FloorStatus> {
  return {
  GroundFloor: 'Available',
  FirstFloorLeft: 'Locked',
  FirstFloorRight: 'Locked',
  SecondFloorLeft: 'Locked',
  SecondFloorRight: 'Locked',
  ThirdFloorLeft: 'Locked',
  ThirdFloorRight: 'Locked',
  TopFloor: 'Locked',
  };
}

@Injectable({ providedIn: 'root' })
export class GameStateService {
  private state$ = new BehaviorSubject<GameProgress>(this.load());

  get progress$() { return this.state$.asObservable(); }
  get snapshot() { return this.state$.value; }

  load(): GameProgress {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) return JSON.parse(raw) as GameProgress;
    } catch {}
    return {
      version: 1,
      floors: defaultFloors(),
      hasReachedPart2: false,
      collectedIngredients: [],
      collectedMagicNumbers: {},
  hasSeenStartupVideo: false,
    };
  }

  save(): void {
    try { localStorage.setItem(STORAGE_KEY, JSON.stringify(this.snapshot)); } catch {}
  }

  setStatus(id: FloorId, status: FloorStatus) {
    const next = { ...this.snapshot, floors: { ...this.snapshot.floors, [id]: status } };
    this.state$.next(next); this.save();
  }

  /** Marks that the startup video has been seen and persists the change. */
  markStartupSeen(): void {
    if (this.snapshot.hasSeenStartupVideo) return;
    const next: GameProgress = { ...this.snapshot, hasSeenStartupVideo: true };
    this.state$.next(next);
    this.save();
  }

  shouldRedirectToFreeMathPage(): boolean {
    const s = this.snapshot;
    const allIngredients = 7; // placeholder constant
    const topDone = s.floors.TopFloor === 'Resolved';
    return s.hasReachedPart2 && !topDone && s.collectedIngredients.length < allIngredients;
  }
}
