import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  template: `
    <div class="home-container">
      <img class="house" src="assets/images/math_house.png" alt="Math House" />
    </div>
  `,
  styles: [`
    .home-container { display:flex; align-items:center; justify-content:center; padding:16px; }
    .house { width: 100%; max-width: 900px; height: auto; user-select:none; }
  `]
})
export class HomeComponent {}
