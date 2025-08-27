import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameLevelService, OperationType } from '../../core/services/game-level.service';
import { GameStateService } from '../../core/services/game-state.service';
import { FloorId } from '../../core/models/enums';

@Component({
  selector: 'app-game-level',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './game-level.component.html',
  styleUrl: './game-level.component.scss'
})
export class GameLevelComponent {
  floorId!: FloorId;
  inputValue = '';
  started = false;
  finished = false;
  correct = 0;
  total = 7;

  constructor(private route: ActivatedRoute, private router: Router, public engine: GameLevelService, private state: GameStateService) {
    const id = (this.route.snapshot.paramMap.get('id') as FloorId) ?? 'GroundFloor';
    this.floorId = id;
    const op = this.mapOpForFloor(id);
    this.engine.init(op, this.total);
  }

  start() { this.started = true; this.engine.start(); }

  mapOpForFloor(id: FloorId): OperationType {
    switch (id) {
      case 'GroundFloor': return 'AddTwoNumbersLessThan10';
      case 'FirstFloorLeft': return 'AddThreeNumbersLessThan10';
      case 'FirstFloorRight': return 'AddTwoNumbersLessThan20';
      case 'SecondFloorLeft': return 'SubtractThreeNumbersLessThan10';
      case 'SecondFloorRight': return 'AddAndSubstract2NumbersLessThan20';
      case 'ThirdFloorLeft': return 'AddAndSubstract3NumbersLessThan10';
      case 'ThirdFloorRight': return 'AddAndSubstract2NumbersLessThan20';
      case 'TopFloor': return 'AddAndSubstract2NumbersLessThan20';
    }
  }

  submit() {
    const num = parseInt(this.inputValue, 10);
    if (isNaN(num)) return;
    const ok = this.engine.submit(num);
    this.inputValue = '';
    const p = this.engine.progress; this.correct = p.correct; this.total = p.total;
    if (this.engine.isOver()) {
      this.finished = true;
      // mark resolved and unlock next floors
      this.state.setStatus(this.floorId, 'Resolved');
      this.unlockNext();
    }
  }

  unlockNext() {
    const order: FloorId[] = ['GroundFloor','FirstFloorLeft','FirstFloorRight','SecondFloorLeft','SecondFloorRight','ThirdFloorLeft','ThirdFloorRight','TopFloor'];
    const dependents: Record<FloorId, FloorId[]> = {
      GroundFloor: ['FirstFloorLeft','FirstFloorRight'],
      FirstFloorLeft: ['SecondFloorLeft'],
      FirstFloorRight: ['SecondFloorRight'],
      SecondFloorLeft: ['ThirdFloorLeft'],
      SecondFloorRight: ['ThirdFloorRight'],
      ThirdFloorLeft: ['TopFloor'],
      ThirdFloorRight: ['TopFloor'],
      TopFloor: [],
    } as const;
    for (const next of dependents[this.floorId] ?? []) {
      // only unlock if currently locked
      if (this.state.snapshot.floors[next] === 'Locked') this.state.setStatus(next, 'Available');
    }
  }

  backHome() { this.router.navigateByUrl('/'); }
}
