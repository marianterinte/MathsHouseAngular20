import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FloorId, FloorStatus } from '../models/enums';
import { GameProgress } from '../models/game-progress';
import { MediaConfig, defaultMediaConfig } from '../models/media-config';

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
  readonly media: MediaConfig = defaultMediaConfig;
  private responsive$ = new BehaviorSubject<any | null>(null);

  get progress$() { return this.state$.asObservable(); }
  get snapshot() { return this.state$.value; }
  get responsiveLayout() { return this.responsive$.value; }

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

  /** Allows overriding media filenames/paths at runtime (e.g., from Settings). */
  setMediaConfig(partial: Partial<MediaConfig>) {
    Object.assign((this as any).media, partial);
  }

  /** Resets the game to the initial state. */
  resetToInitial(options: { preserveIntroSeen?: boolean } = {}) {
    const keepIntro = options.preserveIntroSeen ?? false;
    const next: GameProgress = {
      version: 1,
      floors: defaultFloors(),
      hasReachedPart2: false,
      collectedIngredients: [],
      collectedMagicNumbers: {},
      hasSeenStartupVideo: keepIntro ? this.snapshot.hasSeenStartupVideo : false,
    };
    this.state$.next(next);
    this.save();
  }

  shouldRedirectToFreeMathPage(): boolean {
    const s = this.snapshot;
    const allIngredients = 7; // placeholder constant
    const topDone = s.floors.TopFloor === 'Resolved';
    return s.hasReachedPart2 && !topDone && s.collectedIngredients.length < allIngredients;
  }

  /** Loads responsive layout JSON to drive sizing/positions based on device buckets. */
  async ensureResponsiveLoaded(): Promise<void> {
    if (this.responsive$.value) return;
    try {
      // Prefer files shipped under src/assets, then fall back to mapped external Assets.
      const urls = [
        'assets/Raw/responsive_layout.json',
        'assets/raw/responsive_layout.json',
        'assets/i18n/responsive_layout.json' // unlikely, but keep flexible
      ];
      for (const url of urls) {
        try {
          const res = await fetch(url);
          if (res.ok) {
            const json = await res.json();
            this.responsive$.next(json);
            break;
          }
        } catch {}
      }
    } catch {
      // ignore; fallback CSS will be used
    }
  }
}
