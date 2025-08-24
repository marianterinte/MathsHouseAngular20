import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <header class="app-header">
      <div class="left">
        <ng-content select="[header-title]"></ng-content>
      </div>
      <div class="spacer"></div>
      <div class="right">
        <a class="home-btn" routerLink="/settings" aria-label="Home (Settings)">
          <img class="icon" src="assets/images/home.png" alt="Home icon" />
          <span>Home</span>
        </a>
        <ng-content select="[header-actions]"></ng-content>
      </div>
    </header>
  `,
  styles: [
    `
    .app-header {
      position: sticky; top: 0; z-index: 1000;
      display: flex; align-items: center; gap: 12px;
      padding: 8px 12px; background: #fff; border-bottom: 1px solid #e5e7eb;
    }
    .left { display: flex; align-items: center; gap: 8px; }
    .right { display: flex; align-items: center; gap: 8px; }
    .spacer { flex: 1 1 auto; }
    .home-btn { display: inline-flex; align-items: center; gap: 6px; text-decoration: none; color: #111827; font-weight: 500; padding: 6px 10px; border-radius: 8px; border: 1px solid #e5e7eb; background: #f9fafb; }
    .home-btn:hover { background: #f3f4f6; }
    .home-btn .icon { width: 18px; height: 18px; object-fit: contain; }
    `
  ]
})
export class HeaderComponent {}
