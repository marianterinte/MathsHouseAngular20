import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocalizationService } from '../../core/services/localization.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  langs = [
    { code: 'en', label: 'English' },
    { code: 'de', label: 'Deutsch' },
    { code: 'fr', label: 'Français' },
    { code: 'es', label: 'Español' },
    { code: 'it', label: 'Italiano' },
    { code: 'ro', label: 'Română' },
    { code: 'sv', label: 'Svenska' },
    { code: 'hu', label: 'Magyar' },
    { code: 'ar', label: 'العربية' },
  ];
  selected = 'en';
  sample = '';

  constructor(public i18n: LocalizationService) {
    this.selected = i18n.currentLang || 'en';
    this.sample = i18n.t('main_page_start_message');
  }

  async onLangChange() {
    await this.i18n.setLanguage(this.selected);
    this.sample = this.i18n.t('main_page_start_message');
  }
}
