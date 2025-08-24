import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { GameStateService } from '../services/game-state.service';

export const redirectHomeGuard: CanActivateFn = () => {
  const state = inject(GameStateService);
  const router = inject(Router);
  if (state.shouldRedirectToFreeMathPage()) {
    router.navigateByUrl('/free-math');
    return false;
  }
  return true;
};
