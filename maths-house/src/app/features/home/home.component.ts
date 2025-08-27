import { Component, HostListener, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameStateService } from '../../core/services/game-state.service';
import { MediaConfig } from '../../core/models/media-config';
import { LocalizationService } from '../../core/services/localization.service';
import { TypewriterService } from '../../core/services/typewriter.service';
import { FloorId } from '../../core/models/enums';
import { LoggerService } from '../../core/services/logger.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  showStartup = false;
  media!: MediaConfig;
  vars: Record<string, string> = {};
  @ViewChild('startupVideo') startupVideo?: ElementRef<HTMLVideoElement>;
  @ViewChild('startupAudio') startupAudio?: ElementRef<HTMLAudioElement>;
  autoplayBlocked = false;
  // character message panel
  avatarImage = 'assets/images/puf_wondering.png';
  readonly characterName = 'Puf-Puf';
  gameMessage = '';
  private startTimer: any | null = null;

  constructor(private readonly router: Router, private readonly game: GameStateService, private readonly i18n: LocalizationService, private readonly typer: TypewriterService, private readonly log: LoggerService) {}

  ngOnInit(): void {
    this.media = this.game.media;
    this.showStartup = this.computeShouldShowIntro();
    this.log.info('Home init', { showStartup: this.showStartup, media: this.media, stored: this.safeReadStoredProgress() });
    // Load responsive layout and compute CSS variables
    this.game.ensureResponsiveLoaded().then(() => {
      this.log.debug('Responsive loaded', this.game.responsiveLayout);
      this.computeResponsiveVars();
    });
    // Init localization and set the initial message
    this.initMessage();
  }

  ngAfterViewInit(): void {
    // Attempt autoplay after view is ready
    if (this.showStartup) {
      // Delay to ensure the element is in the DOM
      this.startTimer = setTimeout(() => {
        this.startTimer = null;
        this.tryStartIntro();
      }, 0);
    }
  }

  ngOnDestroy(): void {
    if (this.startTimer) {
      clearTimeout(this.startTimer);
      this.startTimer = null;
    }
  }

  /**
   * Determine if we should show the intro on landing the Home route.
   * Rules: if query ?intro=1 => show; if ?intro=0 => hide.
   * Otherwise: show unless localStorage flag hasSeenStartupVideo is true.
   */
  // (definition moved below)
  

  open(id: FloorId) {
    const status = this.game.snapshot.floors[id];
    if (status === 'Available') {
      this.router.navigate(['/level', id]);
    } else if (status === 'Resolved' && id !== 'TopFloor') {
      // Later: play animal video
    } else if (id === 'TopFloor' && status === 'Resolved') {
      // Later: play wizard video
    }
  }

  isFirstAction(id: FloorId): boolean {
    // Blink for the first available window only (initially FirstFloorLeft)
    const floors = this.game.snapshot.floors;
    const order: FloorId[] = ['FirstFloorLeft','FirstFloorRight','SecondFloorLeft','SecondFloorRight','ThirdFloorLeft','ThirdFloorRight','GroundFloor','TopFloor'];
    const first = order.find(f => floors[f] === 'Available');
    return first === id;
  }

  isLocked(id: FloorId): boolean { return this.game.snapshot.floors[id] === 'Locked'; }
  isResolved(id: FloorId): boolean { return this.game.snapshot.floors[id] === 'Resolved'; }
  badgeText(id: FloorId): string {
    const s = this.game.snapshot.floors[id];
    if (s === 'Available') return '?';
    if (s === 'Resolved') return '✓';
    return '';
  }
  badgeIcon(id: FloorId): string | null {
    const s = this.game.snapshot.floors[id];
    if (s === 'Locked') return 'assets/images/lock.png';
    return null;
  }

  onStartupEnded(): void {
  this.game.markStartupSeen();
  this.showStartup = false;
  this.log.info('Startup ended, intro marked as seen');
  }

  private tryStartIntro(): void {
    const video = this.startupVideo?.nativeElement;
    if (!video) { this.log.warn('Startup video element not available yet'); return; }
    // Ensure properties satisfy autoplay policies
    video.muted = true;
  video.playsInline = true; // iOS Safari
    video.autoplay = true;
    try { video.load(); } catch {}

    const doPlay = () => video.play();
  const p: Promise<void> | undefined = doPlay();
  if (p && typeof (p as any).then === 'function') {
      p.then(() => {
        this.log.info('Startup video autoplayed');
        // Try to play the companion audio (may be blocked; best-effort)
        const audio = this.startupAudio?.nativeElement;
        if (audio) {
          audio.autoplay = false; // we'll start it explicitly
          audio.muted = false;
          audio.currentTime = 0;
          audio.play().catch(err => {
            this.log.warn('Startup audio autoplay blocked (expected on most browsers). Will require user gesture.', err);
          });
        }
      }).catch(err => {
        this.log.warn('Startup video autoplay blocked; showing user gesture fallback', err);
        this.autoplayBlocked = true;
      });
    } else {
      // Older browsers: no promise, assume playing
      this.log.info('Startup video play() returned no promise; assuming playback started');
    }
  }

  onUserStart(): void {
    // Triggered by user gesture to start video/audio
    const video = this.startupVideo?.nativeElement;
    if (video) {
      video.muted = false; // unmute if user explicitly starts
    video.playsInline = true;
      video.play().then(() => {
        this.autoplayBlocked = false;
        this.log.info('Startup video started after user gesture');
        const audio = this.startupAudio?.nativeElement;
        if (audio) {
      audio.currentTime = 0;
          audio.play().catch(err => this.log.warn('Audio still blocked after user gesture', err));
        }
      }).catch(err => this.log.error('Failed to start video on user gesture', err));
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.computeResponsiveVars();
  }

  private computeResponsiveVars(): void {
    this.log.debug('Compute responsive vars');
    type Bucket = 'SmallPhone' | 'MediumPhone' | 'LargePhone' | 'SmallTablet' | 'MediumTablet' | 'LargeTablet';
    interface ResponsiveConf { HouseImageMaxHeight?: number; AnimalSize?: number; CharacterImageSize?: number; FontSizes?: { GameMessage?: number } }
    const layout = this.game.responsiveLayout as Record<Bucket, ResponsiveConf> | null;
    const w = window.innerWidth;
    const bucket = this.pickBucket(w);
    const conf = layout?.[bucket] ?? {};
    const houseMax = conf.HouseImageMaxHeight ? `${conf.HouseImageMaxHeight}px` : '100%';
    const handSize = conf.AnimalSize ?? 80;
    const avatarSize = conf.CharacterImageSize ?? 72;
    const msgFont = conf.FontSizes?.GameMessage ?? 16;
    this.vars = {
      '--house-max-height': houseMax,
      '--hand-size': `${handSize}px`,
      '--avatar-size': `${avatarSize}px`,
      '--msg-font': `${msgFont}px`,
    };
  }

  private pickBucket(width: number): 'SmallPhone' | 'MediumPhone' | 'LargePhone' | 'SmallTablet' | 'MediumTablet' | 'LargeTablet' {
    // Simple width-based bucketing approximating MAUI layout variants
    if (width <= 360) return 'SmallPhone';
    if (width <= 414) return 'MediumPhone';
    if (width <= 540) return 'LargePhone';
    if (width <= 768) return 'SmallTablet';
    if (width <= 1024) return 'MediumTablet';
    return 'LargeTablet';
  }

  private async initMessage(): Promise<void> {
    await this.i18n.init('en');
  this.log.debug('i18n loaded lang', this.i18n.currentLang);
    const end = this.game.snapshot.floors.TopFloor === 'Resolved';
    const full = end
      ? this.i18n.t('main_page_end_message')
      : this.i18n.t('main_page_start_message');
    // Animate with typewriter
    this.typer.start(full, (current) => (this.gameMessage = current), 25);
    this.avatarImage = end ? 'assets/images/puf_succeded.png' : 'assets/images/puf_wondering.png';
  }

  // Media error handlers
  onVideoError(e: Event) { this.log.error('Startup video failed to load', { src: this.media?.startupVideo, event: e }); }
  onAudioError(e: Event) { this.log.error('Startup audio failed to load', { src: this.media?.startupAudio, event: e }); }
  onImageError(kind: string, src: string | null, e: Event) { this.log.error('Image failed to load', { kind, src, event: e }); }

  private safeReadStoredProgress(): any | null {
    try {
      const raw = localStorage.getItem('GameProgress');
      if (!raw) return null;
      return JSON.parse(raw);
    } catch { return null; }
  }

  /**
   * Determine if we should show the intro on landing the Home route.
   * Rules: if query ?intro=1 => show; if ?intro=0 => hide.
   * Otherwise: show unless localStorage flag hasSeenStartupVideo is true.
   */
  private computeShouldShowIntro(): boolean {
    try {
      const params = new URLSearchParams(window.location.search);
      const intro = params.get('intro');
      if (intro === '1') { this.log.info('Intro forced ON via ?intro=1'); return true; }
      if (intro === '0') { this.log.info('Intro forced OFF via ?intro=0'); return false; }
    } catch {}
    const stored = this.safeReadStoredProgress();
    const seen = !!stored?.hasSeenStartupVideo;
    return !seen;
  }
}
