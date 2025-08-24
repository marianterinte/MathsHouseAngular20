import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private dict: Record<string, string> = {};
  private initialized = false;
  currentLang = 'en';

  async init(locale = 'en'): Promise<void> {
    if (this.initialized) return;
    this.currentLang = locale;
    const urls = [
      `assets/raw/i18n.${locale}.json`,
      `assets/Raw/i18n.${locale}.json`,
    ];
    for (const url of urls) {
      try {
        const res = await fetch(url);
        if (res.ok) {
          this.dict = await res.json();
          this.initialized = true;
          return;
        }
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

  t(key: string, fallback?: string): string {
    return this.dict[key] ?? fallback ?? key;
  }
}
