import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  currentLang = 'en';
  setLanguage(lang: string) { this.currentLang = lang; }
  t(key: string, params?: Record<string, any>): string { return key; }
}
