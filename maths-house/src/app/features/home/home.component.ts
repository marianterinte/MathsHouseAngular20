import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameStateService } from '../../core/services/game-state.service';
import { MediaConfig } from '../../core/models/media-config';
import { LocalizationService } from '../../core/services/localization.service';
import { TypewriterService } from '../../core/services/typewriter.service';
import { FloorId } from '../../core/models/enums';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  showStartup = false;
  media!: MediaConfig;
  vars: Record<string, string> = {};
  // character message panel
  avatarImage = 'assets/images/puf_wondering.png';
  characterName = 'Puf-Puf';
  gameMessage = '';

  constructor(private router: Router, private game: GameStateService, private i18n: LocalizationService, private typer: TypewriterService) {}

  ngOnInit(): void {
    this.media = this.game.media;
    this.showStartup = !this.game.snapshot.hasSeenStartupVideo;
    // Load responsive layout and compute CSS variables
    this.game.ensureResponsiveLoaded().then(() => this.computeResponsiveVars());
    // Init localization and set the initial message
    this.initMessage();
  }

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
  }

  @HostListener('window:resize')
  onResize() {
    this.computeResponsiveVars();
  }

  private computeResponsiveVars() {
    const layout = this.game.responsiveLayout;
    const w = window.innerWidth;
    const bucket = this.pickBucket(w);
    const conf = layout?.[bucket];
    const houseMax = conf?.HouseImageMaxHeight ? `${conf.HouseImageMaxHeight}px` : '100%';
    const handSize = conf?.AnimalSize ?? 80;
    const avatarSize = conf?.CharacterImageSize ?? 72;
    const msgFont = conf?.FontSizes?.GameMessage ?? 16;
    this.vars = {
      '--house-max-height': houseMax,
      '--hand-size': `${handSize}px`,
      '--avatar-size': `${avatarSize}px`,
      '--msg-font': `${msgFont}px`,
    };
  }

  private pickBucket(width: number): string {
    // Simple width-based bucketing approximating MAUI layout variants
    if (width <= 360) return 'SmallPhone';
    if (width <= 414) return 'MediumPhone';
    if (width <= 540) return 'LargePhone';
    if (width <= 768) return 'SmallTablet';
    if (width <= 1024) return 'MediumTablet';
    return 'LargeTablet';
  }

  private async initMessage() {
    await this.i18n.init('en');
    const end = this.game.snapshot.floors.TopFloor === 'Resolved';
    const full = end
      ? this.i18n.t('main_page_end_message')
      : this.i18n.t('main_page_start_message');
    // Animate with typewriter
    this.typer.start(full, (current) => (this.gameMessage = current), 25);
    this.avatarImage = end ? 'assets/images/puf_succeded.png' : 'assets/images/puf_wondering.png';
  }
}
