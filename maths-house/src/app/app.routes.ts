import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { GameLevelComponent } from './features/level/game-level.component';
import { FreeMathComponent } from './features/free-math/free-math.component';
import { FreeMathProblemComponent } from './features/free-math/free-math-problem.component';
import { SettingsComponent } from './features/settings/settings.component';
import { AboutComponent } from './features/about/about.component';
import { redirectHomeGuard } from './core/guards/redirect-home.guard';

export const routes: Routes = [
	{ path: '', component: HomeComponent, canActivate: [redirectHomeGuard] },
	// Alias to support direct links like /home without dropping query params
	{ path: 'home', component: HomeComponent, canActivate: [redirectHomeGuard] },
	{ path: 'level/:id', component: GameLevelComponent },
	{ path: 'free-math', component: FreeMathComponent },
	{ path: 'free-math/problem', component: FreeMathProblemComponent },
	{ path: 'settings', component: SettingsComponent },
	{ path: 'about', component: AboutComponent },
	{ path: '**', redirectTo: '' },
];
