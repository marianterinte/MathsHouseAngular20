import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameStateService } from '../../core/services/game-state.service';
import { FloorId, FloorStatus } from '../../core/models/enums';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  showStartup = false;
  constructor(private router: Router, private game: GameStateService) {}

  ngOnInit(): void {
    this.showStartup = !this.game.snapshot.hasSeenStartupVideo;
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
    const next = { ...this.game.snapshot, hasSeenStartupVideo: true };
    (this.game as any).state$.next(next);
    this.game.save();
    this.showStartup = false;
  }
}
