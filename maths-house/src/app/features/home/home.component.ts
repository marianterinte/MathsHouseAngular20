import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameStateService } from '../../core/services/game-state.service';
import { MediaConfig } from '../../core/models/media-config';
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
  constructor(private router: Router, private game: GameStateService) {}

  ngOnInit(): void {
    this.media = this.game.media;
    this.showStartup = !this.game.snapshot.hasSeenStartupVideo;
    // Load responsive layout and compute CSS variables
    this.game.ensureResponsiveLoaded().then(() => this.computeResponsiveVars());
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
    const handSize = conf?.CharacterImageSize ?? conf?.AnimalSize ?? 80;
    this.vars = {
      '--house-max-height': houseMax,
      '--hand-size': `${handSize}px`,
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
}
