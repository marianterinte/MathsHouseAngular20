import { Injectable } from '@angular/core';
import { LoggerService } from './logger.service';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private dict: Record<string, string> = {};
  private initialized = false;
  currentLang = 'en';

  constructor(private log: LoggerService) {}

  private getSavedLang(): string {
    try {
      const l = localStorage.getItem('lang');
      if (l) return l;
    } catch {}
    return 'en';
  }

  async init(locale?: string): Promise<void> {
    if (this.initialized) return;
    this.currentLang = locale ?? this.getSavedLang();
    const urls = [
      `assets/i18n/i18n.${this.currentLang}.json`, // primary location under src/assets
      `assets/raw/i18n.${this.currentLang}.json`,  // external mapped lowercase
      `assets/Raw/i18n.${this.currentLang}.json`,  // external mapped capitalized
    ];
    for (const url of urls) {
      try {
        const res = await fetch(url);
        if (res.ok) {
          this.dict = await res.json();
          this.initialized = true;
          this.log.info('i18n loaded', url);
          try { localStorage.setItem('lang', this.currentLang); } catch {}
          return;
        }
        this.log.warn('i18n not found at', url, res.status);
      } catch {}
    }
    // Provide a few defaults if file not found
    this.dict = {
      main_page_start_message:
        "Puf-Puf: Hello! A wicked witch has locked the house of the cute animals. They are now waiting to be rescued by you... I'm here to help.",
      main_page_end_message:
        "You did it! All animals are safe and the house is unlocked. Great job!",
    };
    this.initialized = true;
  }

  /** Force change language at runtime and persist it. */
  async setLanguage(locale: string): Promise<void> {
    if (!locale || locale === this.currentLang) {
      try { localStorage.setItem('lang', this.currentLang); } catch {}
      return;
    }
    const urls = [
      `assets/i18n/i18n.${locale}.json`,
      `assets/raw/i18n.${locale}.json`,
      `assets/Raw/i18n.${locale}.json`,
    ];
    for (const url of urls) {
      try {
        const res = await fetch(url);
        if (res.ok) {
          this.dict = await res.json();
          this.currentLang = locale;
          try { localStorage.setItem('lang', locale); } catch {}
          this.log.info('i18n language changed', locale, url);
          return;
        }
      } catch {}
    }
    this.log.warn('Failed to change language, keeping previous', locale);
  }

  t(key: string, fallback?: string): string {
    return this.dict[key] ?? fallback ?? key;
  }
}
